namespace OBIS.QC.Core

open GeoAPI.Geometries
open Microsoft.FSharp.Collections
open NetTopologySuite.IO
open NetTopologySuite.Geometries
open NetTopologySuite.Index.Strtree
open NetTopologySuite.Simplify
open ObisDb
open System
open System.Configuration
open System.IO
open System.Linq

module Seq = 
    open System.Collections.Generic

    let paged n (s:'a seq) =
        let rec take n (s:'a IEnumerator) l = 
            if n = 0 || not (s.MoveNext()) then 
                l
            else 
                take (n-1) s (s.Current::l)

        use e = s.GetEnumerator()
        let rec loop () = 
            seq {
                let lst = take n e []
                if lst.Length > 0 then 
                    yield List.rev lst            
                    yield! loop() 
            }
        loop ()

module LandDistance =
    let degToRad = (1.0 / 180.0) * Math.PI |> (*) ;

    let inline point (x:float,y:float) = new Point(x, y)
    let inline coord (x:float,y:float) = new Coordinate(x, y)

    // model implementation
    let inline project centerlon centerlat lon lat =
        // http://mathworld.wolfram.com/AzimuthalEquidistantProjection.html
        // http://www.radicalcartography.net/?projectionref
        let t:float = degToRad lat
        let l:float = degToRad lon
        let t1 = degToRad centerlat // latitude center of projection
        let l0 = degToRad centerlon // longitude center of projection
        let c = Math.Acos ((sin t1) * (sin t) + (cos t1) * (cos t) * (cos (l-l0)))
        let k = c / (sin c)
        let x = k * (cos t) * (sin (l-l0))
        let y = k * (cos t1) * (sin t) - (sin t1) * (cos t) * (cos (l-l0))
        (x, y)

    let inline project_optimized l0 t1 cost1 lon lat =
        // http://mathworld.wolfram.com/AzimuthalEquidistantProjection.html
        // http://www.radicalcartography.net/?projectionref
        let t:float = degToRad lat
        let l:float = degToRad lon
        
        let costcosll0 = (cos t) * (cos (l-l0))
        let sint = sin t
        let sint1 = sin t1
        let c = Math.Acos ((sint1) * (sint) + (cost1) * costcosll0)
        let k = c / (sin c)
        let x = k * (cos t) * (sin (l-l0))
        let y = k * (cost1) * (sint) - (sint1) * costcosll0
        (x, y)

    let buildTransformer (p:IPoint) = 
        let t1 = (1.0 / 180.0) * Math.PI * p.Y
        let l0 = (1.0 / 180.0) * Math.PI * p.X
        let cost1 = cos t1
        fun (c:Coordinate) -> coord (project_optimized l0 t1 cost1 c.X c.Y)
//        fun (c:Coordinate) -> coord (project p.X p.Y c.X c.Y)

    let point0 = new Point(0.0,0.0)

    let azimuthPointDistance (transformer:Coordinate->Coordinate) (geom:IPolygon) = 
        let projected = geom.ExteriorRing.Coordinates |> Array.map transformer
        let polygon = Geometry.DefaultFactory.CreatePolygon(projected)
        let dist = polygon.Distance(point0)
        if dist = 0.0 then
            -1.0 * polygon.ExteriorRing.Distance(point0) * 6371000.0 // earth radius in meters
        else
            dist * 6371000.0 // earth radius in meters

    let buildGeomIndex shapefile = 
        let dr = new ShapefileDataReader(shapefile, GeometryFactory.Default)
        
        let polygonIndex = new STRtree<(string*IPolygon)>()

        while dr.Read() do
            let id = string (dr.Item "id")
            let simplified = DouglasPeuckerSimplifier.Simplify(dr.Geometry, 0.005)
            if simplified.GetType() = typeof<MultiPolygon> then
                let multi = simplified:?>MultiPolygon
                for p in multi.Geometries do
                    polygonIndex.Insert(p.EnvelopeInternal, (id, p:?>IPolygon))    
            else
                polygonIndex.Insert(dr.Geometry.EnvelopeInternal, (id, simplified:?>IPolygon))
            //polygonIndex.Insert(dr.Geometry.EnvelopeInternal, (id, dr.Geometry:?>IPolygon))
        polygonIndex.Build()
        polygonIndex
    
    let toPoint (p:Position) = 
        (p.Id, (new Point(p.Lon, p.Lat)):>IPoint)

    let getGeoms shapefile = 
        seq {
            let dr = new ShapefileDataReader(shapefile, GeometryFactory.Default)
            while dr.Read() do
                let id = string (dr.Item "id")
                yield (id, dr.Geometry:?> IPolygon)
        }

    let buildQueryEnvelopes (p:IPoint) expand= seq {
        let x, y = p.X, p.Y
        let minx = Math.Max(x - expand, -180.0)
        let miny = Math.Max(y - expand, -90.0)
        let maxx = Math.Min(x + expand, 180.0)
        let maxy = (Math.Min(y + expand, 90.0))
        yield new Envelope(minx, maxx, miny, maxy)
        if x - expand < -180.0 then
            yield new Envelope(360.0+(x-expand), 180.0, miny, maxy)
        if x + expand > 180.0 then
            yield new Envelope(-180.0, (x+expand)-360.0, miny, maxy)
    }
        
    let rec getLandDistanceHelper (index:STRtree<(string*IPolygon)>) expand (id:int, point:IPoint) = 
        let geoms = buildQueryEnvelopes point expand
                    |> Seq.map (fun env -> (index.Query(env)))
                    |> Seq.concat
        let f (closest, approx:float) coast = 
            let g:IPolygon = snd coast
            //if (g.IsWithinDistance(point, approx)) then // small shortcut
            if not g.IsEmpty then // distance is 0 for empty polygon
                let simpledist = g.Distance(point)
                if simpledist < approx then
                    (g, simpledist)
                else (closest, approx)
            else
                (closest, approx)
                        
        let mindist = geoms |> Seq.fold f (null, Double.MaxValue)
        if (snd mindist) < expand then
            let trans = buildTransformer point
            {Id=id; LandDistance = (azimuthPointDistance trans (fst mindist)) }
        else
            getLandDistanceHelper index (expand*2.0) (id, point)

    let run positions =
        let coastlines = System.Configuration.ConfigurationManager.AppSettings.["gshhs"]
        let coastlinesIndex = timeit "Build coastlines index" buildGeomIndex coastlines

        positions
        |> List.filter (fun p -> p.Lon <= 180.0 && p.Lon >= -180.0 && p.Lat <= 90.0 && p.Lat >= -90.0)
        |> timeit (sprintf "Get landdistance for %i positions" (List.length positions)) (List.map (toPoint>>(getLandDistanceHelper coastlinesIndex 5.0)))

    let runDb() =
        let conn, positions = timeit "Query positions" ObisDb.queryMissingPositions ObisDb.PositionsTable.Distance 
        positions
        |> Seq.paged 50000
        |> PSeq.collect run
        |> timeit "Copy land distance" ObisDb.copyPositionDistance conn

    // file based version
    let runFiles input output = 
        let toPosition (row:String) = 
            let columns = row.Split('|')
            { Id= (int (columns.[0])); Lon=(float (columns.[1])); Lat=(float (columns.[2])) }
        
        let lines = File.ReadAllLines input |> Seq.skip 1 
        let distances =
            lines
            |> Seq.skip 1 // skip header row
            //|> Seq.take (1*1000) // limit n° of results for testing
            |> Seq.map toPosition
            |> List.ofSeq
            |> Seq.paged 500000
            |> PSeq.map run
            |> Seq.concat
            |> List.ofSeq

        File.WriteAllLines(output, (distances |> (List.map (fun d -> sprintf "%i,%g" (d.Id) (d.LandDistance))))) 
        |> ignore



