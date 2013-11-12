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
open ProjNet.CoordinateSystems
open PositionsDepth

open DotSpatial.Projections

[<AutoOpen>]
module Units =

    open System

    [<Measure>] type km
    [<Measure>] type rad
    [<Measure>] type deg

    let degToRad (degrees : float<deg>) =
        degrees * Math.PI / 180.<deg/rad>

[<AutoOpen>]
module Constants =
    let earthRadius = 6371.<km>

module GreatCircle = 

    open System

    /// Calculates the great-circle distance between two Latitude/Longitude positions on a sphere of given radius.
    let DistanceBetween (radius:float<km>) lat1 long1 lat2 long2 =
        let lat1r, lat2r, long1r, long2r = lat1 |> degToRad, 
                                           lat2 |> degToRad,
                                           long1 |> degToRad,
                                           long2 |> degToRad
        let deltaLat = lat2r - lat1r
        let deltaLong = long2r - long1r

        let a = Math.Sin(deltaLat/2.<rad>) ** 2. +
                (Math.Sin(deltaLong/2.<rad>) ** 2. * Math.Cos((double)lat1r) * Math.Cos((double)lat2r))

        let c = 2. * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.-a))

        radius * c

    /// Calculate DistanceBetween for Earth.
    let DistanceBetweenEarth = DistanceBetween earthRadius


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
            polygon.Distance(point0) * 6371000.0 // meters

    let buildPointIndex points = 
        let index = new Quadtree<(int*IPoint)>()
        for id, (p:IPoint) in points do
            index.Insert(p.EnvelopeInternal, (id,p))
        //index.Build()
        index

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

//    let envelope (point:IPoint) expand = 
//        new Envelope(point.X + expand
//
//    let minDistance (index:STRtree<(string*IPolygon)>) (point:(int*IPoint)) expand =
//        let found = index.Query((snd point).EnvelopeInternal.)
//        c
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
    let processedPoints = new System.Collections.Generic.Dictionary<int, bool>()
    let getLandDistances (coastIndex:STRtree<(string*IPolygon)>) (pointIndex:Quadtree<(int*IPoint)>) initialExpand (id:int, startpoint:IPoint) =
        
        let qpoints = buildQueryEnvelopes startpoint 0.5
                    |> Seq.map (fun env -> (pointIndex.Query(env)))
                    |> Seq.concat
                    |> List.ofSeq

        let rec landDistances expand points =
            let geoms = buildQueryEnvelopes startpoint expand
                        |> Seq.map (fun env -> (coastIndex.Query(env)))
                        |> Seq.concat
                        |> Array.ofSeq
            let rec loop list approxGeom result todo =
                match list with
                | (id,point)::xs -> let folder (closest, approx:float) coast = 
                                        let g:IPolygon = snd coast
                                        let simpledist = g.Distance(point)
                                        if simpledist < approx then // small shortcut
                                            (g, simpledist)
                                        else
                                            (closest, approx)
                                    let mindist = geoms |> Array.fold folder (null, Double.MaxValue)
                                    if (snd mindist) < expand then
                                        let trans = buildTransformer point
                                        loop xs (fst mindist) ((id, (azimuthPointDistance trans (fst mindist)))::result) todo
                                    else
                                        loop xs (fst mindist) result ((id, point)::todo)
                | _ -> result, todo
            let result, todo = loop points null [] []
            if todo = [] then
                result
            else result @ landDistances (expand * 3.0) todo

        for point in qpoints do
            processedPoints.Add(fst point, true)
            pointIndex.Remove((snd point).EnvelopeInternal, point) 
            |> ignore
        //points |> List.iter (fun (id, p) -> (pointIndex.Remove(p.EnvelopeInternal, (id,p)) |> ignore))

        landDistances initialExpand qpoints
        
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
//                let d = azimuthPointDistance trans g
//                if d < real then (d,simpledist) else (real,approx)
                else (closest, approx)
            else
                (closest, approx)
//            else 
//                (real, approx)
                        
        let mindist = geoms |> Seq.fold f (null, Double.MaxValue)
        if (snd mindist) < expand then
            //(id, (fst mindist))
            (id, (azimuthPointDistance trans (fst mindist)))
        else
            getLandDistance index (expand*2.0) (id, point)

    let handlePositions positions coastlines output = 
        let positions = loadPositions positions |> Seq.take 500
        let allpoints = positions |> List.ofSeq |> List.map toPoint

        let coastlinesIndex = buildGeomIndex coastlines
        printfn "coastline geometries loaded"
        let sw = System.Diagnostics.Stopwatch.StartNew()
        
        let minvalues = allpoints |> PSeq.map (getLandDistance coastlinesIndex 5.0)
//
//        let processP points =
//            let pointsIndex = buildPointIndex points
//            //printfn "pointsIndex build"
//        
//            //let coastlineGeoms = Array.ofSeq (getGeoms @"D:\a\data\coastline\GSHHS_i_L1.shp")
//            
//            
//            let minvalues = points |> Seq.map (fun (id, p) -> if processedPoints.ContainsKey(id) then [] else (getLandDistances coastlinesIndex pointsIndex 1.0 (id,p))) |> Seq.concat
//            minvalues
//
//        let minvalues = 
//            let t = allpoints
//                    |> List.mapi (fun i p -> i % 10,p) // split in 10 groups
//                    |> Seq.groupBy fst
//                    |> Seq.map (snd>>(Seq.map snd))
//                    |> List.ofSeq
//            t
//            |> PSeq.map processP
//            |> Seq.concat

        let toLine seq = Seq.map (fun (id, d) -> sprintf "%i,%g" id d) seq
        File.WriteAllLines(output, (toLine minvalues)) |> ignore
        printfn "done in %f s" (sw.Elapsed.TotalSeconds)
        //minvalues |> Seq.take 50 |> Seq.map (printfn "%A") |> List.ofSeq 
        |> ignore


    let handle() =
        handlePositions """D:\temp\all_positions.csv""" @"D:\a\data\coastline\GSHHS_i_L1.shp" """D:\temp\insert_min_distance_positions.csv"""
//
//
//        let g = points.Head;
//
//        minDistance coastlinesIndex g |> ignore
//
//        let c = coastlinesIndex.Query((snd g).EnvelopeInternal).First()
//        (snd c).Distance(snd g)
//
//
//        //((snd c) :?> Polygon)
//        |> ignore
//        //coastlinesIndex
//        
////        let ctf= new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory()
////        let cf = new CoordinateSystemFactory()
////        //let aeqd = cf.CreateFromWkt("+proj=aeqd +lat_0=0 +lon_0=0 +x_0=0 +y_0=0 +ellps=WGS84 +datum=WGS84 +units=m +no_defs")
////        let aeqd = cf.CreateFromWkt("""PROJCS["World_Azimuthal_Equidistant",GEOGCS["GCS_WGS_1984",DATUM["WGS_1984",SPHEROID["WGS_1984",6378137,298.257223563]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]],PROJECTION["Azimuthal_Equidistant"],PARAMETER["False_Easting",0],PARAMETER["False_Northing",0],PARAMETER["Central_Meridian",0],PARAMETER["Latitude_Of_Origin",0],UNIT["Meter",1],AUTHORITY["EPSG","54032"]]""")
////        let ct = ctf.CreateFromCoordinateSystems(GeographicCoordinateSystem.WGS84, aeqd)
////
////        let p1 = project 0.0 0.0 ((snd g).X) ((snd g).Y)
////        let p2 = ct.MathTransform.Transform([|(snd g).X;(snd g).Y|])
//
//
//        //(snd c).
//        let ps =  KnownCoordinateSystems.Geographic.World.WGS1984
//        let pd = ProjectionInfo.FromProj4String "+proj=aeqd +lat_0=0 +lon_0=0 +x_0=0 +y_0=0 +ellps=WGS84 +datum=WGS84 +units=m +no_defs"
//        let xy = [|(snd g).X;(snd g).Y|]
//        Reproject.ReprojectPoints(xy, [|0.0|], ps, pd, 0, 1)
//        let np = new Point(xy.[0], xy.[1])
//        np.Distance(new Point(0.0, 0.0))
//        1


    //let s = 6377781.891 |> (*)
    let s = 6371000.0|> (*)
    let s2 = 1.7960434740193 |> (*)
    let gcd = GreatCircle.DistanceBetweenEarth 0.0<deg> 0.0<deg>

    let comp x y = 
        
        printfn "custom %A" ((s ((point (project 0.0 0.0 x y)).Distance(point (0.0, 0.0)))) / 1000.0)
        printfn "gcd %A" (gcd (x*1.0<deg>) (y*1.0<deg>))
        let xy = [|x;y|]
        let ps =  KnownCoordinateSystems.Geographic.World.WGS1984
        let pd = ProjectionInfo.FromProj4String "+proj=aeqd +lat_0=0 +lon_0=0 +x_0=0 +y_0=0 +ellps=WGS84 +datum=WGS84 +units=m +no_defs"
        Reproject.ReprojectPoints(xy, [|0.0|], ps, pd, 0, 1)
        let np = new Point(xy.[0], xy.[1])
        printfn "dotspatial %A" (s2 (np.Distance(new Point(0.0, 0.0))))




    let arraymapminVsArrayfold() = 
        let x = Array.init 30000 (fun i -> i)
        let sw = System.Diagnostics.Stopwatch.StartNew()
//        for i in 1 .. 20000 do
//            x
//            |> Array.map (1 |> (-))
//            |> Array.min
//            |> ignore
//        printfn "time taken %i" sw.ElapsedMilliseconds
//        sw.Restart()
        for i in 1 .. 20000 do
            x
            |> Array.fold (fun s i -> Math.Min((1 |> (-) i), s)) Int32.MaxValue
            |> ignore
        printfn "time taken %i" sw.ElapsedMilliseconds
        for i in 1 .. 20000 do
            let mutable m = Int32.MaxValue
            for t in x do
                let r = (1 |> (-) t)
                if m < r then 
                    m <- r
        printfn "time taken %i" sw.ElapsedMilliseconds