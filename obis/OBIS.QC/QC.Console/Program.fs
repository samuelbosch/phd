// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System
open OBIS.QC.Core

[<EntryPoint>]
let main argv = 
    //printfn "%A" argv
    if argv.Length = 2 then
        DepthStats.run argv.[0] argv.[1]
    else
        DepthStats.run """D:\temp\tname_depth.csv""" """D:\temp\depth_stats.csv"""

    Console.WriteLine("Hit enter to close")
    Console.ReadLine() |> ignore
    0 // return an integer exit code
