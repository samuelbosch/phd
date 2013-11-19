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
    type SpeciesValue = {
        mutable DrId: int
        mutable PositionId : int
        mutable Value : float
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

    let getMissingPositions (conn:IDbConnection) positionsTable = 
        conn.Query<Position>(sprintf """
        SELECT p.id Id, p.longitude Lon, p.latitude Lat
          FROM obis.positions p 
     LEFT JOIN %s j 
            ON p.id = j.position_id 
         WHERE j.position_id IS NULL""" (fst (getInfo positionsTable)))

    let getSpeciesValue (conn:IDbConnection) positionsTable tnameId =
        let tableName, valueField = getInfo positionsTable
        conn.Query<SpeciesValue>(sprintf """
        SELECT drs.id DrId, p.position_id PositionId, p.%s Value
          FROM %s p
          JOIN obis.drs drs ON drs.position_id = pd.position_id
          JOIN obis.snames sn ON drs.sname_id = sn.id
        WHERE sn.tname_id = %i""" valueField tableName tnameId)
                
