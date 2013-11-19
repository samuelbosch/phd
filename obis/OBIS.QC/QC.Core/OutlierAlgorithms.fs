(* sources:

http://www.eng.tau.ac.il/~bengal/outlier.pdf
http://www.r-bloggers.com/finding-outliers-in-numerical-data/
https://en.wikipedia.org/wiki/Median_absolute_deviation
*)

namespace OBIS.QC.Core.OutlierAlgorithms

open FStats.Statistics
open OBIS.QC.Core.Statistics

module General = 
    let inline filter lower upper = Seq.filter (fun x -> x < lower || x > upper)
    let inline findOutliers treshold center deviation data =
        let lower = center - (treshold * deviation)
        let upper = center + (treshold * deviation)
        data |> filter lower upper

module HampelIdentifier = 
    // uses the median absolute deviation to find outliers
    // with normally distributed data a treshold of 1.4826 is the equivalent of 1 standard deviation
    let findOutliers treshold data = 
        let median, mad = medianMad data
        General.findOutliers treshold median mad data

module EsdIdentifier =
    // extreme Studentized deviation
    // declares any point more than t standard deviations from the mean to be an outlier
    // most common treshold value is 3
    let findOutliers treshold data =
        let mean, sd = meanSd data
        General.findOutliers treshold mean sd data

module BoxplotRule =
    // most common treshold value used is 1.5
    // get Q1, Q2 and IQR (inter quartile range) and calculate the lower and upper bound from this
    // most commmon treshold value is 1.5
    let findOutliers treshold data =
        let q1, q3, iqr = q1q3Iqr data
        let lower = q1 - (treshold * iqr)
        let upper = q3 + (treshold * iqr)
        data |> General.filter lower upper
