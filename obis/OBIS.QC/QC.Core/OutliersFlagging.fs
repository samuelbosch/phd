namespace OBIS.QC.Core

open System
open OBIS.QC.Core.ObisDb
open OBIS.QC.Core.OutlierAlgorithms

module OutliersFlagging =
    type Outlier = { DrId:int; IsHampelOutlier:bool; IsEsdOutlier:bool; IsBoxoutlier:bool }

    let clearOutlierFlag (outlierQc:seq<UnivariateOutlierQC>) (data:seq<SpeciesValue>)  =
        //SELECT (2^2+2^3)::integer # (2^2)::integer => 8
        1        

    let findDepthOutliers (outlierQc:seq<UnivariateOutlierQC>) (data:seq<SpeciesValue>) = 
        let data = Seq.cache data
        let values = data |> Seq.map (fun x -> x.Value)
        let filters = outlierQc
                        |> Seq.map (fun x ->
                            match x.Method with
                            | "hampel" -> x.QcNumber, (HampelIdentifier.createFilter x.Treshold values)
                            | "esd" -> x.QcNumber, (EsdIdentifier.createFilter x.Treshold values)
                            | "box" -> x.QcNumber, (BoxplotRule.createFilter x.Treshold values)
                            | m -> raise (new ArgumentException(sprintf "Unkown outlier method %s" m)))
        let calcQc (x:SpeciesValue) = 
            filters
            |> Seq.sumBy (fun (qcnumber, filter) -> if filter x.Value then int (Math.Pow(2., (float qcnumber))) else 0)
//        seq {
//            for x in data do
//                let qc = calcQc x
//                if qc > 0 then
//                    yield { DrId
//        }
        1
//
//        data
//
//        |> Seq.map 
//            (filters |> Seq.sumBy (qcnumber, filter) -> if filter 
//        
    
