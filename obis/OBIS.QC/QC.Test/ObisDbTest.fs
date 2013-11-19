namespace OBIS.QC.Test

open NUnit.Framework
open FsUnit
open OBIS.QC.Core.ObisDb

[<TestFixture>]
type ``Given ObisDB module``() = 
    
    [<Test>] member x.
     ``When get distance table info then first returns table name`` () = 
        getInfo PositionsTable.Distance |> fst |> should equal "qc.positions_distance"
