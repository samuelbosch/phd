namespace SB.SDM.Pipeline.Pipe

open System.IO
open System
open System.Linq
open Microsoft.FSharp.Collections
open NetTopologySuite.IO
open NetTopologySuite.Geometries
open NetTopologySuite.Index.Strtree
open NetTopologySuite.Index.Quadtree
open GeoAPI.Geometries
open PositionsDepth


module LandDistance =
    let degToRad = (1.0 / 180.0) * Math.PI |> (*) ;

    let inline point (x:float,y:float) = new Point(x, y)
    let inline coord (x:float,y:float) = new Coordinate(x, y)

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

    let inline buildTransformer (p:IPoint) = fun (c:Coordinate) -> coord (project p.X p.Y c.X c.Y)

    let point0 = new Point(0.0,0.0)

    let azimuthPointDistance (transformer:Coordinate->Coordinate) (geom:IPolygon) = 
        let projected = geom.ExteriorRing.Coordinates |> Array.map transformer
        let polygon = Geometry.DefaultFactory.CreatePolygon(projected)
        if polygon.Intersects(point0) then
            0.0
        else
            polygon.Distance(point0) * 6371000.0 // earth radius in meters

    let buildGeomIndex shapefile = 
        let dr = new ShapefileDataReader(shapefile, GeometryFactory.Default)
        
        let polygonIndex = new STRtree<(string*IPolygon)>()

        while dr.Read() do
            let id = string (dr.Item "id")
            polygonIndex.Insert(dr.Geometry.EnvelopeInternal, (id, dr.Geometry:?>IPolygon))
        polygonIndex.Build()
        polygonIndex
    
    let toPoint p = 
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
            yield new Envelope(360.0-(x-expand), 180.0, miny, maxy)
        if x + expand > 180.0 then
            yield new Envelope(-180.0, (x+expand)-360.0, miny, maxy)
    }
        
    let rec getLandDistance (index:STRtree<(string*IPolygon)>) expand (id:int, point:IPoint) = 
        let geoms = buildQueryEnvelopes point expand
                    |> Seq.map (fun env -> (index.Query(env)))
                    |> Seq.concat
        let trans = buildTransformer point
        let f (closest, approx:float) coast = 
            let g:IPolygon = snd coast
            if (g.IsWithinDistance(point, approx)) then // small shortcut
                let simpledist = g.Distance(point)
                if simpledist < approx then
                    (g, simpledist)
                else (closest, approx)
            else
                (closest, approx)
                        
        let mindist = geoms |> Seq.fold f (null, Double.MaxValue)
        if (snd mindist) < expand then
            (id, (azimuthPointDistance trans (fst mindist)))
        else
            getLandDistance index (expand*2.0) (id, point)

    let handlePositions positions coastlines output = 
        let positions = loadPositions positions
        let allpoints = positions |> List.ofSeq |> List.map toPoint

        let coastlinesIndex = buildGeomIndex coastlines
        printfn "coastline geometries loaded"
        let sw = System.Diagnostics.Stopwatch.StartNew()
        
        let minvalues = allpoints |> PSeq.map (getLandDistance coastlinesIndex 5.0)

        let toLine seq = Seq.map (fun (id, d) -> sprintf "%i,%g" id d) seq
        File.WriteAllLines(output, (toLine minvalues)) |> ignore
        printfn "done in %f s" (sw.Elapsed.TotalSeconds)
        |> ignore


    let handle() =
        handlePositions """D:\temp\all_positions.csv""" @"D:\a\data\coastline\GSHHS_i_L1.shp" """D:\temp\insert_min_distance_positions.csv"""


    let splitPositions() =
        let c =  500000
        let lines = File.ReadAllLines("""D:\temp\all_positions.csv""")
        let header = lines.[0]
        let mutable index  = 1 // skip first row
        let a = Array.zeroCreate (c+1)
        a.[0] <- header
        while index+c < lines.Length do
           Array.Copy(lines, index, a, 1, c)
           File.WriteAllLines(sprintf """D:\temp\splitted_positions_%i.csv""" (index/c), a)
           index <- index + c
        let a = Array.zeroCreate ((lines.Length - index)+1)
        a.[0] <- header
        Array.Copy(lines, index, a, 1, a.Length-1)
        File.WriteAllLines(sprintf """D:\temp\splitted_positions_%i.csv""" (index/c), a)
           
         
            
         

