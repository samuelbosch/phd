namespace OBIS.QC.Test

open NUnit.Framework
open FsUnit
open OBIS.QC.Core.Statistics

[<TestFixture>]
type ``Given seq (1, 1, 2, 2, 4, 6, 9)``() = 
    let data = [1.;1.;2.;2.;4.;6.;9.]

    [<Test>] member x.
     ``When medianMad`` () = 
        medianMad data
        |> should equal (2., 1.)

    [<Test>] member x.
     ``When meanSd then mean ok`` () = 
        meanSd data
        |> fst
        |> should (equalWithin 0.02) 3.57
    
    [<Test>] member x.
     ``When meanSd then sd ok`` () = 
        meanSd data
        |> snd
        |> should (equalWithin 0.02) 2.99

    [<Test>] member x.
     ``When q1q3Iqr`` () = 
        q1q3Iqr data
        |> should equal (1.,6.,5.)
