namespace OBIS.QC.Core

open System
open System.Configuration
open System.IO
open System.Linq
open ObisDb


module PositionsRaster =
    let toPixel range pixels endValue value = 
        let pixel = 1 + (int (Math.Truncate((value + (range/2.0)) * (float pixels / range))))
        match pixel with
            | r when r = pixels+1 -> endValue // handle case of +180.0 and +90.0 (returns pixels + 1)
            | _ -> pixel

    let getValues (positions:ObisDb.Position []) (lines:seq<String>) =
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

        let colNumber (p:ObisDb.Position) = int (toPixel 360.0 colCount 1 p.Lon) - 1 // 0 based
        let lineNumber (p:ObisDb.Position) = int (toPixel 180.0 rowCount rowCount (-1.0*p.Lat)) - 1 // 0 based
        let toIdLineCol (p:ObisDb.Position) = (p.Id, (lineNumber p), (colNumber p))
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

