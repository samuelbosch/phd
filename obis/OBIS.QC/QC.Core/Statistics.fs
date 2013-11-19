namespace OBIS.QC.Core

module Statistics =
    open FStats.Statistics
    
    // median and median absolute deviation
    let inline medianMad data = 
        let dataMedian = median data
        let deviation = Seq.map (((-) dataMedian) >> abs) data
        let mad = (median deviation)
        dataMedian, mad

    // mean and sample standard deviation
    let inline meanSd data =
        let mean = mean data
        let sd = 
            data
            |> Seq.sumBy (fun x -> pown (x - mean) 2)
            |> (*) (1. / (Seq.length data - 1 |> float))
            |> sqrt
        mean, sd

    let inline q1q3Iqr data = 
        let q1 = quartile 1 data
        let q3 = quartile 3 data
        let iqr = q3 - q1 // interquartile range
        q1, q3, iqr

