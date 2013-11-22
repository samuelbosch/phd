namespace OBIS.QC.Core

open FStats.Statistics
open OBIS.QC.Core.Statistics
open System
open System.IO

module Tuple =
    let toArray = Microsoft.FSharp.Reflection.FSharpValue.GetTupleFields

module File = 
    let writeAllLines path (contents:seq<string>) = 
        File.WriteAllLines(path, contents)

module DepthStats =
    let applystatfuncs (data:float []) = 
        let inv d f = f d
        let c:obj->float option[] = Tuple.toArray >> Array.map (fun x -> x :?> (float option))
        let x = [|(meanSd>>c); (medianMad>>c); (q1q3Iqr>>c)|]
        let t = x |> Array.collect (inv data)
        t

    let statistics (data:seq<int*float>) =
        data 
        |> Seq.map (snd)
        |> Array.ofSeq
        |> applystatfuncs
        |> Array.map (fun s -> match s with 
                               | Some(s) -> string s 
                               | None -> """\N""") // NULL

    let load input = // making more ram friendly by making load a separate function
        File.ReadAllLines(input)
        |> Seq.skip 1 // header
        |> Seq.map ((fun l -> l.Split([|','|])) >> (fun x -> (int x.[0]) , (float x.[1])))
        |> Array.ofSeq

    let run input output = 
        load input
        |> Seq.groupBy fst
        |> Seq.map (fun (tnameid, data) -> 
                        let stats = statistics data
                        sprintf "%i,%i,%s" tnameid (Seq.length data) (String.Join(",", stats)))
        |> File.writeAllLines output
        |> ignore