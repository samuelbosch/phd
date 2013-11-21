namespace OBIS.QC.Core

module Statistics =
    open FStats.Statistics
    
    // median and median absolute deviation
    let inline medianMad data = 
        let dataMedian = median data
        if Seq.length data > 1 then
            let deviation = Seq.map (((-) dataMedian) >> abs) data
            let mad = (median deviation)
            Some(dataMedian), Some(mad)
        else
            Some(dataMedian), None

    // mean and sample standard deviation
    let inline meanSd data =
        let mean = mean data
        if Seq.length data > 1 then
            let sd = 
                data
                |> Seq.sumBy (fun x -> pown (x - mean) 2)
                |> (*) (1. / (Seq.length data - 1 |> float))
                |> sqrt
            Some(mean), Some(sd)
        else
            Some(mean), None

    let inline q1q3Iqr data = 
        if Seq.length data > 3 then
            let q1 = quartile 1 data
            let q3 = quartile 3 data
            let iqr = q3 - q1 // interquartile range
            Some(q1), Some(q3), Some(iqr)
        else
            None, None, None

