(* sources:
http://www.eng.tau.ac.il/~bengal/outlier.pdf
http://www.r-bloggers.com/finding-outliers-in-numerical-data/
https://en.wikipedia.org/wiki/Median_absolute_deviation
*)

namespace OBIS.QC.Core.OutlierAlgorithms

open FStats.Statistics
open OBIS.QC.Core.Statistics

module General = 
    let inline buildPredicate lower upper x = x < lower || x > upper
    let inline createFilter treshold data (centerDevStat:seq<float>->float option*float option) =
        match centerDevStat data with
        | Some(center), Some(deviation) -> 
            let lower = center - (treshold * deviation)
            let upper = center + (treshold * deviation)
            buildPredicate lower upper
        | _ -> (fun x -> false)

    let inline findOutliers treshold data (centerDevStat:seq<float>->float option*float option) =
        data |> Seq.filter (createFilter treshold data centerDevStat)
    
module HampelIdentifier = 
    // uses the median absolute deviation to find outliers
    // with normally distributed data a treshold of 1.4826 is the equivalent of 1 standard deviation
    let findOutliers treshold data = 
        General.findOutliers treshold data medianMad

    let createFilter treshold data = 
        General.createFilter treshold data medianMad

module EsdIdentifier =
    // extreme Studentized deviation
    // declares any point more than t standard deviations from the mean to be an outlier
    // most common treshold value is 3
    let findOutliers treshold data =
        General.findOutliers treshold data meanSd

    let createFilter treshold data = 
        General.createFilter treshold data meanSd

module BoxplotRule =
    let createFilter treshold data =
        match q1q3Iqr data with
        | Some(q1),Some(q3), Some(iqr) -> 
            let lower = q1 - (treshold * iqr)
            let upper = q3 + (treshold * iqr)
            General.buildPredicate lower upper
        | _ -> (fun x -> false)

    // most common treshold value used is 1.5
    // get Q1, Q2 and IQR (inter quartile range) and calculate the lower and upper bound from this
    // most commmon treshold value is 1.5
    let findOutliers treshold data =
        data |> Seq.filter (createFilter treshold data)
