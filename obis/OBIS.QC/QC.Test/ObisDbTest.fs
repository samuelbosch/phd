namespace OBIS.QC.Test

open NUnit.Framework
open FsUnit
open OBIS.QC.Core
open ObisDb
open Dapper
open System
open System.Configuration
open System.Data

[<CLIMutable>]
type TestObjectWithArray = {
    mutable Arr: int []
}

[<TestFixture>]
type ``Given ObisDB module``() = 
    
    [<Test>] 
    member x.``When get distance table info then first returns table name`` () = 
        getInfo PositionsTable.Distance |> fst |> should equal "qc.positions_distance"

    [<Test>] 
    member x.``When get query with array then Dapper supports arrays`` () = 
        let conn = connect()
        let d = conn.Query<TestObjectWithArray>("SELECT Array[1] Arr") |> Seq.head
        d.Arr |> should equal [|1|]
