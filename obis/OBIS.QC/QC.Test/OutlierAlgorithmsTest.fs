namespace OBIS.QC.Test

open NUnit.Framework
open FsUnit
open OBIS.QC.Core.OutlierAlgorithms

[<TestFixture>]
type ``Given Hampel identifier module``() = 
    
    [<Test>] member x.
     ``When find outliers for seq (1, 1, 2, 2, 4, 6, 9) and treshold 3 then 6 and 9 found`` () = 
        
        HampelIdentifier.findOutliers 3. [1.0;1.0;2.0;2.0;4.0;6.0;9.0] 
        |> List.ofSeq |> should equal [6.0;9.0]

[<TestFixture>]
type ``Given ESD identifier module``() = 
    
    [<Test>] member x.
     ``When find outliers for seq (1, 1, 2, 2, 4, 6, 9) and treshold 1 then 9 found`` () = 
        
        EsdIdentifier.findOutliers 1. [1.0;1.0;2.0;2.0;4.0;6.0;9.0] 
        |> List.ofSeq |> should equal [9.0]

[<TestFixture>]
type ``Given boxplot rule module``() = 
    
    [<Test>] member x.
     ``When find outliers for seq (1, 1, 2, 2, 4, 6, 9) and treshold 0.5 then 9 found`` () = 
        
        BoxplotRule.findOutliers 0.5 [1.0;1.0;2.0;2.0;4.0;6.0;9.0] 
        |> List.ofSeq |> should equal [9.0]
