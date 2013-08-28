namespace SB.SDM.Pipeline.Pipe

open System.Collections.Generic
open System

open Dapper
open OutlierAlgorithms

module Outliers =    
    
    let private conn = DB.connect()

    let private getPoints tnameId = 
        
        let points = conn.Query<Presence>(sprintf """SELECT id, lat, lon, tname_id, resource_id FROM p.presences WHERE tname_id = %d""" tnameId)
        let result = points |> List.ofSeq|> List.map (fun p -> new Point(p.lon, p.lat, p.id))
        printfn "Fetched %d records for species with id %d" result.Length tnameId
        result
//
//    let private pointsToValues clusterId (points:Point list) =
//        let values = 
//            points 
//            |> List.map (fun p -> (sprintf "(%d, %d)" p.Id.Value clusterId))
//            |> String.concat ","
//        values
//
//    let private insertValues (conn:Npgsql.NpgsqlConnection) values =
//        if values <> String.Empty then
//            let insertHeader = "INSERT INTO p.presences_qa  (presence_id, cluster_id) VALUES "
//            conn.Execute(insertHeader + values, commandTimeout=Nullable(0)) |> ignore

    
    let private updateValues (conn:Npgsql.NpgsqlConnection) tnameId clusterId (points:Point list) =
        let clusterId = if clusterId < 0 then -1 else clusterId+1 // passed in clusterId is 0 based but one based looks better in the DB
        if points.Length > 0 then
            let ps = points |> List.map (fun p -> p.Id.Value.ToString()) |> String.concat ","
            let sql = sprintf "UPDATE p.presences SET qa_cluster_id = %d WHERE tname_id=%d AND id IN (%s)" clusterId tnameId ps
            conn.Execute(sql, commandTimeout=Nullable(0)) |> ignore

    let detectOutliers tnameId = 
        let points = getPoints tnameId
        let maxdistance = 2500.0
        let minPoints = 5
        let clusters = DBSCAN points maxdistance minPoints
        printfn "cluster count %d" clusters.Count

        // insert presences without clusters
        let noisePoints = points |> List.filter (fun p -> p.Noise)
        printfn "noise count %d" noisePoints.Length
        
        updateValues conn tnameId -1 noisePoints

        // update cluster ids
        clusters
        |> Seq.map List.ofSeq
        |> Seq.iteri (updateValues conn tnameId)

//        let values = noisePoints |> (pointsToValues -1 )
//        insertValues conn values
//
//        // insert cluster ids
//        clusters
//        |> Seq.mapi (fun i cluster -> (pointsToValues (i+1) (List.ofSeq cluster)))
//        |> Seq.iter (insertValues conn)

    let test = 
        let points = [new Point(0.0, 100.0);
                      new Point(0.0, 200.0);
                      (new Point(0.0, 275.0));
                      (new Point(100.0, 150.0));
                      (new Point(200.0, 100.0));
                      (new Point(250.0, 200.0));        
                      (new Point(0.0, 300.0));        
                      (new Point(100.0, 200.0));
                      (new Point(600.0, 700.0));        
                      (new Point(650.0, 700.0));        
                      (new Point(675.0, 700.0));        
                      (new Point(675.0, 710.0));        
                      (new Point(675.0, 720.0));        
                      (new Point(50.0, 400.0))]
        let clusters = DBSCAN points 100.0 3
        for point in points do
            if point.Noise && not point.InCluster then
                printfn "noise %f %f" point.Lon point.Lat
        let mutable counter = 0
        for cluster in clusters do
            counter <- counter + 1
            printfn "Cluster %d consists of the following %d point(s)" counter cluster.Count
            for point in cluster do
                printf "(%f, %f) " point.Lon point.Lat
            printfn ""


        
//    let regionQuery (p:Point) eps =
//        [(new Point())] 
//
//    let rec x neighborPts C eps minPts =
//        for p' in neighborPts do
//            if not p'.Visited do
//                p'.Visited <- true
//                let neighborPts' = regionQuery p' eps
//                if List.length neighborPts' >= minPts do
//                    let C' =  x
//        C
//    let expandCluster P neighborPts C eps minPts =
//        let C = P::C

        
        
        
                

//    let dbscan D eps minPts = 
//        
//        for p in D do
//            p.Visited <- true
//            let neighborPts = regionQuery p eps
//            if (List.length neighborPts) < minPts do
//                p.Noise <- true
//                option.None
//            else
//                let C = expandCluster p neighborPts eps minPts
//                option.Some(C)
//
//        0
