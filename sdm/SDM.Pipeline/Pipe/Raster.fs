namespace SB.SDM.Pipeline.Pipe
(*
#r @"D:\a\Google Drive\code\sdm\SDM.Pipeline\packages\FSPowerPack.Core.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.dll";;
*)
open System.IO
open System.Collections.Generic
open System.Linq
open System
open Dapper
open MathNet.Numerics.LinearAlgebra

module Seq =
    let mapi64 f s = seq {
        let i64 = ref 0L
        for item in s do
            yield (f i64.Value item)
            i64 := !i64 + 1L
    }

    (* Loads ascii raster files into a matrix *)
module Raster =    
    type Raster = { Header:Dictionary<String, String>; Rows: seq<int*int*float> }

    let parseFloat (x:string)  = 
        try 
            let result = float x
            result
        with
            | ex -> printfn "ERROR [%s]" x ; -9999.0

    let splitToFloat (x:string) = Array.map parseFloat (x.TrimEnd([|' '; '\n'|]).Split([|' '|]))

    let ijv i (row:float [])= Seq.mapi (fun j v -> (i,j,v)) row

    let loadAscii p = 
        let lines = File.ReadLines(p)
        let header = lines.Take(6).ToDictionary((fun (l:string) -> l.Split(" ".ToCharArray()).[0]), (fun (l:string) -> l.TrimEnd([|'\n'|]).Split(" ".ToCharArray()).Last()))
        let rows = Seq.mapi (fun i line -> (ijv i (splitToFloat line))) (Seq.cast (lines.Skip(6))) |> Seq.concat
        {Header=header; Rows=rows}

    let sparseMatrix (r:Raster) = 
        let nodata = (parseFloat r.Header.["NODATA_value"])
        Matrix.initSparse (Int32.Parse(r.Header.["nrows"])) (Int32.Parse(r.Header.["ncols"])) (r.Rows |> Seq.filter (fun (_,_,v) -> v <> nodata)) // remove nodata

    let denseMatrix (r:Raster) = Matrix.initDense (Int32.Parse(r.Header.["nrows"])) (Int32.Parse(r.Header.["ncols"])) r.Rows

    let fakeMatrix (r:Raster) = 
        let m = System.Array.CreateInstance(typedefof<System.Double>, [| Int64.Parse(r.Header.["nrows"]); Int64.Parse(r.Header.["ncols"]) |])
        for x,y, value in r.Rows do
            m.SetValue(value, x,y)
        m

    let loadFile p = 
        let dense = loadAscii p |> denseMatrix
        dense

    let loadFileSparse p = 
        let sparse = loadAscii p |> sparseMatrix
        sparse

    let grid = Array2D.init<float> 4320 2160 (fun i j -> 0.1+((float)i)+((float)j))



module RasterValue = 

    let serialize path values = 
        let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        use stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None)
        formatter.Serialize(stream, values)
        stream.Close()
        values

    let deserialize path = 
        let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        use stream = new FileStream(path, FileMode.Open, FileAccess.Read)
        let values = formatter.Deserialize(stream)
        stream.Close()
        values

    let duration f = 
        let watch = System.Diagnostics.Stopwatch.StartNew()
        let x = f()
        printfn "Duration %s" (watch.Elapsed.ToString())
        x

    let getOccurrencePixels (conn:Npgsql.NpgsqlConnection) cacheDir = 
        let cachePath = Path.Combine(cacheDir, "occurrence_pixels.cache")
        
        // re-use cache file when exists and not older then 7 days
        if File.Exists(cachePath) then //&& (DateTime.Now - File.GetCreationTime(cachePath).Date).Days > 7 then
            printfn "using cached occurrence pixels file"
            duration (fun () -> (deserialize cachePath) :?> (OccurrencePixel []))
        else
            printfn "querying occurrence pixels"
            duration (fun () ->
                File.Delete(cachePath)
                let occurrencePixels = conn.Query<OccurrencePixel>("""
    WITH t AS (
      SELECT ST_SETSRID(ST_MakeEmptyRaster(4320, 2160, -180.00000000, 90.00000000, 0.08333333), 4326) rast
    )
      SELECT s.id species_id, pr.tname_id, ST_World2RasterCoordX(rast,ST_X(pr.geom::geometry)) As pixel_x, ST_World2RasterCoordY(rast, st_y(pr.geom::geometry)) as pixel_y
        FROM p.presences pr
        JOIN t ON 1=1
        JOIN p.species s ON s.tname_id = pr.tname_id
       GROUP BY s.id, pr.tname_id, pixel_x, pixel_y
                """, commandTimeout= Nullable(0)) |> Seq.toArray
                serialize cachePath occurrencePixels)


    
    let value (r:matrix) pixelX pixelY = 
        let mutable iX = pixelX - 1
        let mutable iY = pixelY - 1
        if iX = r.NumCols then iX <- 0
        if iY = r.NumRows then iY <- 0

        r.[iY, iX]
         
    let result oracleId (r:matrix) (o:OccurrencePixel) = 
        let v = value r o.pixel_x o.pixel_y
        (string) (oracleId, o.species_id, o.tname_id, v, o.pixel_x, o.pixel_y)
        
        

    let insertSpeciesOracle filename filedir cacheDir = 
        
        let conn = DB.connect()

        let rid = conn.Query<int>(sprintf """
SELECT rid FROM p.oracle_global_rv WHERE filename = '%s'
            """ filename) |> Seq.head

        printfn "Load raster"
        let r = Raster.loadFile (Path.Combine(filedir, filename))

        let occurrencePixels = getOccurrencePixels conn cacheDir

        printfn "Retained %d occurrences" occurrencePixels.LongLength

//        let insertHeader = """INSERT INTO p.species_oracle (oracle_id, species_id, tname_id, cellvalue, pixel_x, pixel_y) VALUES """
//        
//        System.Threading.Thread.CurrentThread.CurrentCulture <- System.Globalization.CultureInfo.InvariantCulture
//
//        for pixels in (Seq.windowed 50000 occurrencePixels) do
//            printfn "Start inserting next batch"
//            let values = 
//                pixels 
//                |> Seq.map (result rid r)
//                |> String.concat ","
//            conn.Execute(insertHeader + values, commandTimeout=Nullable(0)) |> ignore
        // check already inserted
        let alreadyprocessed = conn.Query<int>(sprintf "SELECT oracle_id FROM p.species_oracle WHERE oracle_id = %d LIMIT 1" rid) |> Seq.isEmpty |> not
        if alreadyprocessed then
            printfn "Values for file %s where already uploaded" filename
            0
        else
            let mutable nextId = conn.Query<int>("SELECT (CASE WHEN MAX(id) IS NULL THEN 0 ELSE MAX(id) END)::integer c FROM p.species_oracle") |> Seq.head
            let copyPath = Path.Combine(cacheDir, "occurrences.copy")
            File.Delete(copyPath)

            use writer = new StreamWriter(copyPath)
            for pixel in occurrencePixels do
                nextId <- nextId + 1
                let v = value r pixel.pixel_x pixel.pixel_y
                if v <> -9999.0 then
                    let data = [|(string nextId); (string rid) ; (string pixel.species_id) ; (string pixel.tname_id); (string v); (string pixel.pixel_x); (string pixel.pixel_y); "\\N" |]
                    writer.WriteLine(data |> String.concat "\t")
            
            writer.Close()
        
        
            conn.Execute("COPY p.species_oracle FROM '" + copyPath + "'", commandTimeout=Nullable(0))



