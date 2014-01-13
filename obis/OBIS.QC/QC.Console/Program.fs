// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System
open OBIS.QC.Core

[<EntryPoint>]
let main argv = 
    //printfn "%A" argv
    if argv.Length > 0 then
        if argv.[0].ToLower() = "depthstats" then
            if argv.Length = 1 then DepthStats.runDb()
            else if argv.Length = 3 then DepthStats.run argv.[1] argv.[2]
            else printfn "Invalid depthstats options, pass no arguments or [inputfile] [outputfile]"
        else if argv.[0].ToLower() = "landdistance" then
            if argv.Length = 1 then LandDistance.runDb()
            else if argv.Length = 3 then LandDistance.runFiles argv.[1] argv.[2]
            else printfn "Invalid landdistance options, pass no arguments or [inputfile] [outputfile]"
    else
        DepthStats.run """D:\temp\tname_depth.csv""" """D:\temp\depth_stats.csv"""
        //LandDistance.runFiles """D:\temp\all_positions.csv""" """D:\temp\landdistance_positions.csv"""
    
    

    Console.WriteLine("Hit enter to close")
    Console.ReadLine() |> ignore
    0 // return an integer exit code
