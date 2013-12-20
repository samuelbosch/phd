namespace OBIS.QC.Core

open System
open System.Configuration
open System.IO
open System.Linq
open ObisDb
open PositionsRaster

module PositionsDepth =
    // converts a list of sequences to a sequence of lists
    // when one sequence is exhausted any remaining elements in the other sequences are ignored
    // sample usage:
    // printfn "%A" (zipseq [seq {1 .. 3}; seq {1 .. 4}; seq{1 .. 3}])
    let zipseq (sequencelist:list<seq<'a>>) = 
        let enumerators = sequencelist |> List.map (fun (s:seq<'a>) -> (s.GetEnumerator()))
        seq {
            let hasNext() = enumerators |> List.exists (fun e -> not (e.MoveNext())) |> not
            while hasNext() do
                yield enumerators |> List.map (fun e -> e.Current)
        }

    let toMinMaxAvgConsensusValues (values:(int*float option) list) = 
        let id = fst values.Head
        let values = values |> List.choose snd 
        let consensus = 
                values
                |> List.sort 
                |> List.ofSeq
                |> Seq.windowed 2
                |> Seq.map (fun l -> ((abs (abs l.[0]) - (abs l.[1])),(l.[0]+l.[1])/2.)) // absolute difference, average of 2
                |> Seq.minBy fst // min absolute difference
                |> snd // average
        let min = List.min values
        let max = List.max values
        let avg = List.sum values / (float (List.length values))

        {Id=id;MinDepth=min;MaxDepth=max;AvgDepth=avg;Consensus=consensus}

    let getDepths positions =
        let handle (rasterType:string) = 
            let path = System.Configuration.ConfigurationManager.AppSettings.[rasterType]
            let lines = File.ReadLines(path)
            timeit ("Get values " + rasterType) (getValues positions) lines

        let valuesPerRaster = List.map handle ["gebco";"etopo";"marspec"]
        
        let zippedValues = zipseq valuesPerRaster
        zippedValues

    let runDb() = 
        let conn, positions = timeit "Query positions" ObisDb.queryMissingPositions ObisDb.PositionsTable.Depth 
        positions
        |> Array.ofSeq
        |> timeit "Get depths" getDepths
        |> Seq.map toMinMaxAvgConsensusValues
        |> timeit "Copy postion depths" ObisDb.copyPositionDepth conn