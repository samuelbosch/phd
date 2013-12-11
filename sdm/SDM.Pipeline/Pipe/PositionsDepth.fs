namespace SB.SDM.Pipeline.Pipe

open System.IO
open System
open System.Globalization
open System.Configuration
open System.Linq
open Microsoft.FSharp.Collections
open RasterCompressed

module PositionsDepth =

//    [<CLIMutable>]
//    type Position = {
//        mutable id : int
//        mutable lon : float
//        mutable lat : float
//    }

    type Position = { Id:int; Lon:float; Lat:float }

    let toPosition (row:String) = 
        let columns = row.Split('|')
        { Id= (int (columns.[0])); Lon=(float (columns.[1])); Lat=(float (columns.[2])) }

    let loadPositions file = 
        let r = File.ReadAllLines file
                |> Seq.skip 1 // skip header row
                |> Seq.map toPosition
                |> Seq.filter (fun p -> p.Lon <= 180.0 && p.Lon >= -180.0 && p.Lat <= 90.0 && p.Lat >= -90.0)
        printfn "loaded positions"
        r

    let toPixel range pixels endValue value = 
        let pixel = 1 + (int (Math.Truncate((value + (range/2.0)) * (float pixels / range))))
        match pixel with
            | r when r = pixels+1 -> endValue * 1<px> // handle case of +180.0 and +90.0 (returns pixels + 1)
            | _ -> pixel * 1<px>

    let pixelY info position = toPixel 180.0 info.RowCount info.RowCount (-1.0*position.Lat)
    let pixelX info position = toPixel 360.0 info.ColumnCount 1 position.Lon

    let toIdxy info position = 
        let x = pixelX info position
        let y = pixelY info position
        (position.Id, x,y)
    
    let isValidIdxy info (id, x:int<px>, y:int<px>) = 
        y > 0<px> && x > 0<px> && (int y) <= info.RowCount && (int x) <= info.ColumnCount

    let getRasterValues positions (info,data) =
        let getRasterValue ((x,y),seq) = 
            let v = RasterCompressed.getValue info data x y
            Seq.map (fun (id,x,y) -> (id,v)) seq

        positions 
        |> Seq.map (toIdxy info)
        |> Seq.filter (isValidIdxy info)
        |> Seq.groupBy (fun (id,x,y) -> (x,y))
        |> PSeq.collect getRasterValue // get values for all ids
        |> Seq.sortBy fst // sort by id

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


    let toMinValues (values:(int*float option) list) = 
        let minAcc (_, currentValue) (id, newValue) = 
            match newValue with 
            | Some(v) when v < currentValue -> (id, v) 
            | _ -> (id, currentValue)
        List.fold minAcc (0, Double.MaxValue) values

    let toMaxValues (values:(int*float option) list) = 
        let maxAcc (_, currentValue) (id, newValue) = 
            match newValue with 
            | Some(v) when v > currentValue -> (id, v) 
            | _ -> (id, currentValue)
        List.fold maxAcc (0, Double.MinValue) values

    let toMinMaxValues (values:(int*float option) list) = 
        let minMaxAcc (_, currentMin, currentMax) (id, newValue) = 
            match newValue with 
            | Some(v) when v < currentMin && v > currentMax -> (id, v, v) 
            | Some(v) when v < currentMin -> (id, v, currentMax) 
            | Some(v) when v > currentMax -> (id, currentMin, v) 
            | _ -> (id, currentMin, currentMax)
        List.fold minMaxAcc (0, Double.MaxValue, Double.MinValue) values

    let toMinMaxAvgValues (values:(int*float option) list) = 
        let minMaxAvgAcc (_, currentMin, currentMax, sum, count) (id, newValue) =
            match newValue with 
            | Some(v) when v < currentMin && v > currentMax -> (id, v, v,sum+v, count+1) 
            | Some(v) when v < currentMin -> (id, v, currentMax,sum+v, count+1) 
            | Some(v) when v > currentMax -> (id, currentMin, v,sum+v, count+1) 
            | Some(v) -> (id, currentMin, currentMax,sum+v, count+1) 
            | None -> (id, currentMin, currentMax,sum,count)
        let id, min, max, sum, count = List.fold minMaxAvgAcc (0, Double.MaxValue, Double.MinValue, 0., 0) values
        id, min, max, (sum/(float count))

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

        id, min, max, avg, consensus
    
    let toGebcoEtopoMarspec (values:(int*float option) list) =
        let id = fst values.Head
        let values = values |> List.map snd
        id, values.[0], values.[1], values.[2]

//    let getDepth positions rasterpath = 
//        let pixelY rowCount position = toPixel 180.0 rowCount rowCount (-1.0*position.Lat)
//        let pixelX columnCount position = toPixel 360.0 columnCount 1 position.Lon
//
//        let toIdxy info position = 
//            let x = pixelX info position
//            let y = pixelY info position
//            (position.Id, x,y)
//        let r = Raster.loadAscii rasterpath
//        positions
//        |> Seq.map (toIdxy r.Header.)
//        |> Seq.filter (isValidIdxy info)
//        |> Seq.groupBy (fun (id,x,y) -> (x,y))
//        |> PSeq.collect getRasterValue // get values for all ids
//        |> Seq.sortBy fst // sort by id
//        //r.Rows |> Seq.skip 3205*21601 |> Seq.skip 3448 |> Seq.head;;
//
//    let handlePositions2() = 
//        let positions = loadPositions """D:\temp\all_positions.csv""" |> List.ofSeq
//        let rasters = ["""D:\a\data\gebco\gebco_gridone.asc""";"""D:\a\data\etopo\etopo1_ice_c.asc""";"""D:\a\data\marspec\MARSPEC_30s\ascii\bathy_30s.asc"""]
//        let results = List.map (getDepth positions) rasters

    let getValuesForRaster positions rasterpath = 
        let raster = OutlierAlgorithms.timeit ("load " + rasterpath) (RasterCompressed.loadAscii rasterpath) 1.0
        getRasterValues positions raster

    let getZippedValues positions = 
        let valuesPerRaster = ["""D:\a\data\gebco\gebco_gridone.asc""";"""D:\a\data\etopo\etopo1_ice_c.asc""";"""D:\a\data\marspec\MARSPEC_30s\ascii\bathy_30s.asc"""]
                            //|> Seq.take 1 |> List.ofSeq
                            |> Seq.map (getValuesForRaster positions)
                            |> List.ofSeq
        
        let zippedValues = zipseq valuesPerRaster
        zippedValues

    let handlePositions() = 
        let positions = loadPositions """D:\temp\all_positions.csv""" //|> Seq.take 10000 // first positions for testing
        //let positions = positions |> Seq.filter (fun p -> p.Id = 13281194)
        // calculate raster x, y
        // get value from the 3 rasters
        // update value in DB
        
        //File.WriteAllLines("""D:\temp\all_depths.txt""", (zippedValues |> Seq.map (sprintf "%A")))
        let zippedValues = getZippedValues positions
        let minvalues = zippedValues |> Seq.map toMinValues
        let grouped = minvalues |> Seq.groupBy snd
        let idstr seq = String.Join(",", (Seq.map (fst>>string) seq))
        let update (depth, seq) = sprintf "UPDATE obis.positions SET mindepth = %f WHERE id IN (%s);" depth (idstr seq)
        
        let lines = grouped |> Seq.map update |> List.ofSeq (* trigger sequence execution till the end *)
        File.WriteAllLines("""D:\temp\min_depth_positions.sql""", lines)
        lines |> Seq.take 10 |> List.ofSeq |> List.map (printfn "%s") |> ignore
        
(*

    let assertp a b =
        if not (a = b) then 
            printfn "false got %i but expected %i" a b 
        else 
            printfn "true"
        assert (a = b)

    let testy() =
        let info = { RowCount=2160; ColumnCount=4320; ValueScaleFactor=1.0; Nodata="-32768" }
        let y value = pixelY info {Lon=0.0; Lat=value;Id=1}
        assertp (y -90.0) 2160<px>
        assertp (y -89.999) 2160<px>
        assertp (y -25.001) 1381<px>
        assertp (y -0.001) 1081<px>
        assertp (y 0.00) 1081<px>
        assertp (y 0.001) 1080<px>
        assertp (y 25.001) 780<px>
        assertp (y 89.999) 1<px>
        assertp (y +90.0) 1<px>

    let testx() =
        let info = { RowCount=2160; ColumnCount=4320; ValueScaleFactor=1.0; Nodata="-32768" }
        let x value = pixelX info {Lon=value; Lat=0.0;Id=1}
        assertp (x -180.0) 1<px>
        assertp (x -179.999) 1<px>
        assertp (x -75.999) 1249<px>
        assertp (x -25.999) 1849<px>
        assertp (x -0.001) 2160<px>
        assertp (x 0.0) 2161<px>
        assertp (x 0.001) 2161<px>
        assertp (x 25.999) 2472<px>
        assertp (x 179.999) 4320<px>
        assertp (x 180.0) 1<px>



*)
(*
#I @"D:\a\Google Drive\code\sdm\SDM.Pipeline\Pipe\bin\Release"
#r "Pipe"
open SB.SDM.Pipeline.Pipe
open PositionsDepth
#time "on"
*)

    let getValues (positions:Position []) path =
        let lines = File.ReadLines(path)
        let isHeader (l:string) = (l.Length < 1000)
        let header = lines.TakeWhile(isHeader).ToDictionary((fun (l:string) -> l.Split([|' '|], StringSplitOptions.RemoveEmptyEntries).[0]), (fun (l:string) -> l.TrimEnd([|'\n'|]).Split([|' '|]).Last()))

        let rowCount = int header.["nrows"]
        let colCount = int header.["ncols"]
        let mutable mnodata = ""
        if not (header.TryGetValue("NODATA_value", &(mnodata))) then 
            mnodata <- "-99999"
        let nodata = mnodata

        let inline splitLine (x:string) = x.Trim([|' '; '\n'|]).Split([|' '|], StringSplitOptions.RemoveEmptyEntries)
        let values = lines.SkipWhile(isHeader)

        let linesE = values.GetEnumerator()

        let colNumber position = int (toPixel 360.0 colCount 1 position.Lon) - 1 // 0 based
        let lineNumber position = int (toPixel 180.0 rowCount rowCount (-1.0*position.Lat)) - 1 // 0 based
        let toIdLineCol position = (position.Id, (lineNumber position), (colNumber position))
        let idLineCol = 
            positions 
            |> Array.map toIdLineCol
            |> Array.filter (fun (id, line,col) -> line >= 0 && col >= 0 && col < colCount && line < rowCount)
            |> Array.sortBy (fun (_,l,c) -> l,c)
        
        let values = seq {
            let i = ref -1
            let current = ref (Array.zeroCreate colCount)
            for (id,line,col) in idLineCol do
                while !i < line && linesE.MoveNext() do
                    i := !i+1
                    if !i = line then
                        current := (splitLine linesE.Current) // only split when we need the line

                if !i = line then
                    let v = (!current).[col]
                    if v = nodata then
                        yield (id, None)
                    else
                        yield (id, Some(float v))
        }
        values |> List.ofSeq |> List.sortBy fst |> Seq.ofList // we need a sequence but we don't want to keep the raster in memory

    let getZippedValues2 positions = 
        let valuesPerRaster = ["""D:\a\data\gebco\gebco_gridone.asc""";"""D:\a\data\etopo\etopo1_ice_c.asc""";"""D:\a\data\marspec\MARSPEC_30s\ascii\bathy_30s.asc"""]
                            //|> Seq.take 1 |> List.ofSeq
                            |> Seq.map (fun rasterpath -> OutlierAlgorithms.timeit ("get values " + rasterpath) (getValues positions) rasterpath)
                            |> List.ofSeq
        
        let zippedValues = zipseq valuesPerRaster
        zippedValues

    let handlePositions2() = 
        let positions = loadPositions """D:\temp\all_positions.csv""" //|> Seq.take 10000 // first positions for testing
        //let positions = positions |> Seq.filter (fun p -> p.Id = 13281194)
        // calculate raster x, y
        // get value from the 3 rasters
        // update value in DB
        
        //File.WriteAllLines("""D:\temp\all_depths.txt""", (zippedValues |> Seq.map (sprintf "%A")))
        let zippedValues = getZippedValues2 (positions |> Array.ofSeq)
        let minvalues = zippedValues |> Seq.map toMinValues |> List.ofSeq
//        let grouped = minvalues |> Seq.groupBy snd
//        let idstr seq = String.Join(",", (Seq.map (fst>>string) seq))
//        let update (depth, seq) = sprintf "UPDATE obis.positions SET mindepth = %f WHERE id IN (%s);" depth (idstr seq)
//        
//        let lines = grouped |> Seq.map update |> List.ofSeq (* trigger sequence execution till the end *)
//        File.WriteAllLines("""D:\temp\min_depth_positions.sql""", lines)
//        lines |> Seq.take 10 |> List.ofSeq |> List.map (printfn "%s") |> ignore

        //let toValues seq = String.Join(",", (Seq.map (fun (id, d) -> sprintf "(%i,%g)" id d) seq))
        //let inserts =  sprintf "INSERT INTO qc.positions_depth (id, mindepth) VALUES %s;" (toValues minvalues)
        //File.WriteAllLines("""D:\temp\insert_min_depth_positions.sql""", [|"DELETE FROM qc.positions_depth;";  inserts|]) |> ignore

        let toLine seq = Seq.map (fun (id, d) -> sprintf "%i,%g" id d) seq
        File.WriteAllLines("""D:\temp\insert_min_depth_positions.csv""", (toLine minvalues)) |> ignore


    let handlePositions2MinMax() = 
        let positions = loadPositions """D:\temp\all_positions.csv"""
        let zippedValues = getZippedValues2 (positions |> Array.ofSeq)

        let minmaxvalues = zippedValues |> Seq.map toMinMaxValues |> List.ofSeq
        let toLine seq = Seq.map (fun (id, min,max) -> sprintf "%i,%g,%g" id min max) seq
        File.WriteAllLines("""D:\temp\insert_min_max_depth_positions.csv""", (toLine minmaxvalues)) |> ignore

    let handlePositions2MinMaxAvg() = 
        let positions = loadPositions """D:\temp\all_positions.csv"""
        let zippedValues = getZippedValues2 (positions |> Array.ofSeq)

        let values = zippedValues |> Seq.map toMinMaxAvgValues |> List.ofSeq
        let toLine seq = Seq.map (fun (id, min,max,avg) -> sprintf "%i,%g,%g,%g" id min max avg) seq
        File.WriteAllLines("""D:\temp\insert_min_max_avg_depth_positions.csv""", (toLine values)) |> ignore

    let handlePositions2MinMaxAvgConsensus() = 
        let positions = loadPositions """D:\temp\all_positions.csv"""
        let zippedValues = getZippedValues2 (positions |> Array.ofSeq)

        let values = zippedValues |> Seq.map toMinMaxAvgConsensusValues |> List.ofSeq
        let toLine seq = Seq.map (fun (id, min,max,avg,consensus) -> sprintf "%i,%g,%g,%g,%g" id min max avg consensus) seq
        File.WriteAllLines("""D:\temp\insert_min_max_avg_consensus_depth_positions.csv""", (toLine values)) |> ignore

    let handlePositions2exact() = 
        let positions = loadPositions """D:\temp\all_positions.csv""" //|> Seq.take 1000 // first positions for testing
        //let positions = seq { yield { Id=13192822; Lon=(-105.125); Lat=19.5666675567627 } |> Seq.ofList
        //-74.0678558349609 11.3256063461304
        let zippedValues = getZippedValues2 (positions |> Array.ofSeq)

        let values = zippedValues |> Seq.map toGebcoEtopoMarspec |> List.ofSeq
        let str x = if Option.isSome x then string x.Value else """NULL"""
        let toLine seq = Seq.map (fun (id, gebco,etopo,marspec) -> sprintf "%i,%s,%s,%s" id (str gebco) (str etopo) (str marspec)) seq
        File.WriteAllLines("""D:\temp\insert_exact_depth_positions.csv""", (toLine values)) |> ignore