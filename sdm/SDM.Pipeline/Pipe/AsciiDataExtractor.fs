namespace SB.SDM.Pipeline.Pipe

open System.IO
open System
open System.Linq

module AsciiDataExtractor =
    type ParsedCsv = { Data:string; Lon:float; Lat:float }


    let toPixel range pixels endValue value = 
        let pixel = 1 + (int (Math.Truncate((value + (range/2.0)) * (float pixels / range))))
        match pixel with
            | r when r = pixels+1 -> endValue * 1<px> // handle case of +180.0 and +90.0 (returns pixels + 1)
            | _ -> pixel * 1<px>

    let getValues (sites:ParsedCsv []) path =
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

        let colNumber site = int (toPixel 360.0 colCount 1 site.Lon) - 1 // 0 based
        let lineNumber site = int (toPixel 180.0 rowCount rowCount (-1.0*site.Lat)) - 1 // 0 based
        let toIdLineCol site = (site.Data, (lineNumber site), (colNumber site))
        let idLineCol = 
            sites
            |> Array.map toIdLineCol
            |> Array.filter (fun (id, line,col) -> line >= 0 && col >= 0 && col < colCount && line < rowCount)
            |> Array.sortBy (fun (_,l,c) -> l,c)

        let getvalue (row:string [] ref) index = 
            if index < 0 || index >= (Array.length (!row)) then
                None
            else
                let v = (!row).[index]
                if v = nodata then 
                    None 
                else 
                    Some(float v)

        let values = seq {
            let i = ref -1
            let current = ref (Array.zeroCreate colCount)
            for (id,line,col) in idLineCol do
                while !i < line && linesE.MoveNext() do
                    i := !i+1
                    if !i = line then
                        (splitLine linesE.Current) 
                        |> Array.mapi (fun i v -> (!current).[i] <- v) 
                        |> ignore // only split when we need the line

                        
                if !i = line then
                    
                    let values = Array.choose (getvalue current) [|col;col-1;col+1|]
                    if values.Length = 0 then
                        yield (id, None)
                    else
                        yield (id, Some(values.[0]))
        }
        values |> List.ofSeq |> Seq.ofList // we need a sequence but we don't want to keep the raster in memory

    let parseCsv (line:string)= 
        let s = line.Split(',')
        { Data=s.[0]; Lat=(float s.[1]); Lon=(float s.[2]) }
    
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
    
    let toOutputLine (l:list<string*float option>) = 
        let site = fst l.Head
        l
        |> List.map (fun (_,v) -> match v with | Some(x) -> (string x) | None -> "NULL")
        |> Array.ofList
        |> fun a -> site + "," + String.Join(",", a)
        

    let run input rasters output =
        let inputLines = File.ReadAllLines(input)
            
        let inputPoints = 
            inputLines 
            |> Seq.skip 1 
            |> Seq.map parseCsv
            |> Array.ofSeq

        let rasters = 
            Directory.EnumerateFiles(rasters, "*.asc")
            |> List.ofSeq
        
        let newHeader = 
            (inputLines |> Seq.head |> fun s -> s.Split(',').[0]) + ","  + (rasters |> List.map (fun p -> Path.GetFileNameWithoutExtension(p)) |> fun a -> String.Join(",", a))

        let values = 
            rasters
            |> List.map (getValues inputPoints)
            |> zipseq
            |> Seq.map toOutputLine
            |> Seq.append [newHeader]
            |> Array.ofSeq

        File.WriteAllLines(output, values)
