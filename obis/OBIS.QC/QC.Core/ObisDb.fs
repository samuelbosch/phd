namespace OBIS.QC.Core

open Dapper
open System
open System.Configuration
open System.Data

module ObisDb =
    type PositionsTable =
    | Depth = 1
    | Distance = 2

    [<CLIMutable>]
    type Position = {
        mutable Id : int
        mutable Lon : float
        mutable Lat : float
    }

    [<CLIMutable>]
    type TnameDepth = {
        mutable Id : int
        mutable Depths : float []
    }

    type PositionDepth = {
        Id:int;
        MinDepth:float;
        MaxDepth:float;
        AvgDepth:float;
        Consensus:float
    }

    type PositionDistance = {
        Id:int;
        LandDistance:float;
    }

    type DepthStats= {
        Id:int;
        Count:int;
        Stats:float option []
    }

    let timeit fname f v = 
        let watch = System.Diagnostics.Stopwatch.StartNew()
        let res = f v 
        watch.Stop()
        Console.WriteLine(sprintf "Needed %f ms for '%s'" (watch.Elapsed.TotalMilliseconds) fname)
        res

    let getInfo positionsTable = 
        match positionsTable with
        | PositionsTable.Depth -> "qc.positions_depth", "depth"
        | PositionsTable.Distance -> "qc.positions_distance", "landdistance"
        | _ -> raise (new ArgumentException("invalid PositionsJoinTable enum value"))

    let connect() = 
        let connString = ConfigurationManager.ConnectionStrings.["DB"].ConnectionString
        let conn = new Npgsql.NpgsqlConnection(connString)
        conn.Open()
        conn

    let queryMissingPositions positionsTable = 
        let conn = connect()
        conn, conn.Query<Position>(sprintf """
        SELECT p.id Id, p.longitude Lon, p.latitude Lat
          FROM obis.positions p 
     LEFT JOIN %s j 
            ON p.id = j.id 
         WHERE j.id IS NULL""" (fst (getInfo positionsTable)))

    let queryTnameDepths (conn:IDbConnection) =
        conn.Query<TnameDepth>(
            """SELECT tname_id Id, Array_Agg(consensus) Depths
                 FROM obis.drs drs
                 JOIN obis.snames sn ON sn.id = drs.sname_id
                 JOIN qc.positions_depth pd ON pd.id = drs.position_id
                 JOIN qc.positions_distance pdist ON pdist.id = drs.position_id
                WHERE (NULLIF(longitude,0) IS NOT NULL AND NULLIF(latitude,0) IS NOT NULL) -- QC 4
                  AND ((longitude BETWEEN -180 AND 180) AND (latitude BETWEEN -90 AND 90)) -- QC 5
                  AND (mindepth <= 0 AND landdistance > -20000) -- QC 6
             GROUP BY tname_id""")
    
    let internal copy (conn:Npgsql.NpgsqlConnection) table serializeRow (data:seq<'a>) = 
        let copyTxt = sprintf "COPY %s FROM STDIN" table
        let serializer = new Npgsql.NpgsqlCopySerializer(conn)
        let copyIn = new Npgsql.NpgsqlCopyIn(copyTxt, conn)
        copyIn.Start()
        data |> Seq.iter (fun t -> (serializeRow serializer t)
                                   serializer.EndRow()
                                   serializer.Flush())
        copyIn.End()
        serializer.Close()

    let copyDepthStats (conn:Npgsql.NpgsqlConnection) stats =
        let serialize (ser:Npgsql.NpgsqlCopySerializer) (stat:DepthStats) =
            ser.AddInt32(stat.Id)
            ser.AddInt32(stat.Count)
            stat.Stats |> Array.iter (fun f-> match f with 
                                              | Some(x) -> ser.AddNumber(x) 
                                              | None -> ser.AddNull())
        copy conn "qc.depth_statistics" serialize stats

    let copyPositionDepth (conn:Npgsql.NpgsqlConnection) depths =
        let serialize (ser:Npgsql.NpgsqlCopySerializer) (d:PositionDepth) =
            ser.AddInt32(d.Id)
            ser.AddNumber(d.MinDepth)
            ser.AddNumber(d.MaxDepth)
            ser.AddNumber(d.AvgDepth)
            ser.AddNumber(d.Consensus)
        copy conn "qc.positions_depth" serialize depths

    let copyPositionDistance (conn:Npgsql.NpgsqlConnection) distances =
        let serialize (ser:Npgsql.NpgsqlCopySerializer) (d:PositionDistance) =
            ser.AddInt32(d.Id)
            ser.AddNumber(d.LandDistance)
        copy conn "qc.positions_distance" serialize distances
