namespace SB.SDM.Pipeline.Pipe

open System.IO
open System
open System.Globalization
open System.Configuration
open RasterCompressed

module PositionsDepth =

//    [<CLIMutable>]
//    type Position = {
//        mutable id : int
//        mutable lon : float
//        mutable lat : float
//    }

    type Position = { Id:int; Lon:float; Lat:float }

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

    let toPosition (row:String) = 
        let columns = row.Split('|')
        { Id= (int (columns.[0])); Lon=(float (columns.[1])); Lat=(float (columns.[2])) }

    let loadPositions file = 
        let r = File.ReadAllLines file
                |> Seq.skip 1 // skip header row
                |> Seq.map toPosition
        printfn "loaded positions"
        r

    let getRasterValues positions (info,data) =
        let getRasterValue ((x,y),seq) = 
            let v = RasterCompressed.getValue info data x y
            Seq.map (fun (id,x,y) -> (id,v)) seq

        positions 
        |> Seq.map (toIdxy info)
        |> Seq.groupBy (fun (id,x,y) -> (x,y))
        |> Seq.collect getRasterValue // get values for all ids
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

    let handlePositions() = 
        let positions = loadPositions """D:\temp\all_positions.csv""" //|> Seq.take 1 // first 100 for testing
        
        // calculate raster x, y
        // get value from the 3 rasters
        // update value in DB
        let rasters = ["""D:\a\data\gebco\gebco_gridone.asc""";"""D:\a\data\etopo\etopo1_ice_c.asc"""; """D:\a\data\marspec\MARSPEC_30s\ascii\bathy_30s.asc"""]
                      //|> Seq.take 1 |> List.ofSeq
                      |> List.map (fun p -> OutlierAlgorithms.timeit ("load " + p) (RasterCompressed.loadAscii p) 1.0)
        
        printfn "loaded rasters"
        
        let valuesPerRaster = List.map (getRasterValues positions) rasters
        let zippedValues = zipseq valuesPerRaster
        
        let minvalues = zippedValues |> Seq.map toMinValues
        let grouped = minvalues |> Seq.groupBy snd
        let idstr seq = String.Join(",", (Seq.map (fst>>string) seq))
        let update (depth, seq) = sprintf "UPDATE obis.positions SET mindepth = %f WHERE id IN (%s)" depth (idstr seq)
        
        let lines = grouped |> Seq.map update |> List.ofSeq (* trigger sequence execution till the end *)
        File.WriteAllLines("""D:\temp\min_depth_positions.sql""", lines)
        lines |> List.map (printfn "%s") |> ignore
        
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