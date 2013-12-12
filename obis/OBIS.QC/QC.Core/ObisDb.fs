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

    type DepthStats= {
        Id:int;
        Count:int;
        Stats:float option []
    }

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

    let copyDepthStats (conn:Npgsql.NpgsqlConnection) (stats:seq<DepthStats>) =
        let serialize (ser:Npgsql.NpgsqlCopySerializer) (stat:DepthStats) =
            ser.AddInt32(stat.Id)
            ser.AddInt32(stat.Count)
            stat.Stats |> Array.iter (fun f-> match f with 
                                              | Some(x) -> ser.AddNumber(x) 
                                              | None -> ser.AddNull())
        copy conn "qc.depth_statistics" serialize stats

    let copyPositionDepth (conn:Npgsql.NpgsqlConnection) (depths:seq<PositionDepth>) =
        let serialize (ser:Npgsql.NpgsqlCopySerializer) (d:PositionDepth) =
            ser.AddInt32(d.Id)
            ser.AddNumber(d.MinDepth)
            ser.AddNumber(d.MaxDepth)
            ser.AddNumber(d.AvgDepth)
            ser.AddNumber(d.Consensus)
        copy conn "qc.positions_depth" serialize depths

    let timeit fname f v = 
        let watch = System.Diagnostics.Stopwatch.StartNew()
        let res = f v 
        watch.Stop()
        printfn "Needed %f ms for %s" (watch.Elapsed.TotalMilliseconds) fname
        res