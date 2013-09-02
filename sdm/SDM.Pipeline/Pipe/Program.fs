namespace SB.SDM.Pipeline.Pipe

open System.Configuration
open System.Collections.Generic
open System

module Program =   
    
    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        
        // arguments ? http://stevegilham.blogspot.be/2011/01/using-f-20-powerpack-argparser-from-c.html

        // Dapper: https://code.google.com/p/dapper-dot-net/

        //Export.allSpeciesToCsv "D:\\a\\data\\obis_db\\species_export\\"

        // TODO investigate duplicate tnames
//        select t1.id, t2.id, t1.*, t2.* from obis.tnames t1, obis.tnames t2
//        WHERE t1.tname = t2.tname
//        AND t1.id <> t2.id
        
//        GC.Collect()
//        printfn "total memory %d" (GC.GetTotalMemory(true))
//        let count f = 
//            //let dense = f @"D:\a\data\BioOracle_GLOBAL_RV\sst_min.asc"
//            let m = f @"D:\a\data\marspec\MARSPEC_30s\ascii\bathy_30s.asc"
//            printfn "total memory %d" (GC.GetTotalMemory(true))
//            printfn "counted %d nodata values" (m |> Seq.filter (fun v -> v = -32768.0) |> Seq.length)
//        count Raster.loadFile
//        printfn "total memory %d" (GC.GetTotalMemory(true))

//        count Raster.loadFileSparse
//        printfn "total memory %d" (GC.GetTotalMemory(true))


//        for layer in ["sst_mean.asc";"sst_max.asc";"sal_mean_an.asc"; "da_min.asc"; "da_max.asc";"da_mean.asc"] do
//            let watch = System.Diagnostics.Stopwatch.StartNew()
//            RasterValue.insertSpeciesOracle layer @"D:\a\data\BioOracle_GLOBAL_RV"  @"D:\temp\obis" |> ignore
//            printfn "%s finished in %s time" layer (watch.Elapsed.ToString())

//        Outliers.test
//        Outliers.test2
//        Earth.test
        //Chart.main()

//        let watch = System.Diagnostics.Stopwatch.StartNew()
//        [395409]//;395413;395439;395450;395457;395460;395465;395927;396052;395781;395808;395810;395915;395966;396016;396197;396205;396237;396249;396314]
//        |> List.iter Outliers.detectOutliers
//
//        printfn "finished outlier detection in %s time" (watch.Elapsed.ToString())

        //let occurrencePixels = RasterValue.getOccurrencePixels null @"D:\temp\obis"
        //printfn "%d pixels" occurrencePixels.LongLength

        RasterCompressed.test()
        //RasterCompressed.benchmark()
        RasterCompressed.benchmarkBigFile()
       //RasterCompressed.benchmark_marspec_10m()

        // main entry point return
        Console.WriteLine("Hit key to close")
        Console.ReadLine() |> ignore
        0