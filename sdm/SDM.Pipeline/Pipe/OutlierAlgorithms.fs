namespace SB.SDM.Pipeline.Pipe

open System.Collections.Generic
open System

module OutlierAlgorithms = 
    type Point(lon,lat, ?id:int) = 
        [<DefaultValue>] val mutable InCluster : bool
        [<DefaultValue>] val mutable Visited : bool
        [<DefaultValue>] val mutable Noise : bool
        let toRad deg = deg * (Math.PI / 180.0)
        member this.Id = id
        member this.Lon = lon
        member this.Lat = lat
        
        member this.LonRad = toRad lon
        member this.LatRad = toRad lat

    let private getDistance (p1:Point) (p2:Point) =
        let diffX = p2.Lon - p1.Lon
        let diffY = p2.Lat - p1.Lat
        let d = diffX * diffX + diffY * diffY
        d

    let memoize f = 
      let cache = System.Collections.Generic.Dictionary<_,_>(HashIdentity.Structural)
      fun x ->
        let ok, res = cache.TryGetValue(x)
        if ok then res
        else let res = f x
             cache.[x] <- res
             res

    let timeit fname f v = 
        let watch = System.Diagnostics.Stopwatch.StartNew()
        let res = f v 
        watch.Stop()
        printfn "Needed %f ms for %s" (watch.Elapsed.TotalMilliseconds) fname
        res

    let private greatCircleDistance (p1:Point) (p2:Point) =
        // code adapted from http://www.codeproject.com/Articles/12269/Distance-between-locations-using-latitude-and-long
        (*
            The Haversine formula according to Dr. Math.
            http://mathforum.org/library/drmath/view/51879.html
                
            dlon = lon2 - lon1
            dlat = lat2 - lat1
            a = (sin(dlat/2))^2 + cos(lat1) * cos(lat2) * (sin(dlon/2))^2
            c = 2 * atan2(sqrt(a), sqrt(1-a)) 
            d = R * c
                
            Where
                * dlon is the change in longitude
                * dlat is the change in latitude
                * c is the great circle distance in Radians.
                * R is the radius of a spherical Earth.
                * The locations of the two points in 
                    spherical coordinates (longitude and 
                    latitude) are lon1,lat1 and lon2, lat2.
        *)
        
        let dlon = p2.LonRad - p1.LonRad;
        let dlat = p2.LatRad - p1.LatRad;

        // Intermediate result a.
        let a = (sin (dlat / 2.0)) ** 2.0 + ((cos p1.LatRad) * (cos p2.LatRad) * (sin (dlon / 2.0)) ** 2.0);

        // Intermediate result c (great circle distance in Radians).
        let c = 2.0 * (asin (sqrt a));

        // Distance.
        // const Double kEarthRadiusMiles = 3956.0;
        let earthRadiusKms = 6371.0;
        let distance = earthRadiusKms * c;

        distance;


    let private getRegion P points eps =
        let region = points |> List.filter (fun p -> (greatCircleDistance P p) <= eps)
        new System.Collections.Generic.List<Point>(region)
    
    let private expandCluster points P (neighborPts:List<Point>) (C:List<Point>) eps minPts = 
        
        C.Add(P)
        P.InCluster <- true

        while neighborPts.Count > 0 do
            let last = neighborPts.Count-1
            let P' = neighborPts.[last]
            neighborPts.RemoveAt(last)

            if not P'.Visited then
                P'.Visited <- true
                let neighborPts' = getRegion P' points eps
                if neighborPts'.Count >= minPts then
                    neighborPts.AddRange(neighborPts')
            if not P'.InCluster then
                C.Add(P')
                P'.InCluster <- true


    let DBSCANuntimed points eps minPts = 
        let clusters = new List<List<Point>>()
        for (p:Point) in points do
            if not p.Visited then
                p.Visited <- true            
                let neighborPts = getRegion p points eps
    
                if neighborPts.Count < minPts then
                    p.Noise <- true
                else
                    let C = new List<Point>()
                    expandCluster points p neighborPts C eps minPts
                    if C.Count > 0 then clusters.Add(C)
        clusters

    let DBSCAN points eps minPts =
        timeit "DBSCAN" (DBSCANuntimed points eps) minPts

    let test2 = 
        let d = greatCircleDistance (new Point(5.0, -32.0)) (new Point(-3.0, 4.0))
        printfn "%f" d // 4091 km

