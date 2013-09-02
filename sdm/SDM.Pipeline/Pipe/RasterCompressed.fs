namespace SB.SDM.Pipeline.Pipe
(*
#r @"D:\a\Google Drive\code\sdm\SDM.Pipeline\packages\FSPowerPack.Core.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.dll";;
*)
open System.IO
open System.Collections.Generic
open System.Collections
open System.Linq
open System
open MathNet.Numerics.LinearAlgebra

open me.lemire.integercompression

[<Measure>] type px


(*
    
Full flow:

1) iterate trough all rows
2) for each row -> turn values into integers
3) create bitarray of lenght column count
4) set true when row value is not NODATA
5) put all real value in an array
6) create an int16 offset array (value = previous - value) + store first value
7) compress the result array

For value retrieval:

1) get bitarry for row
2) check if it is not NODATA
3) uncompress the result array
4) replace offset array with values array
5) get the value from the array (index => count number of bits upto the column_index)
6) convert integers back to the correct value (e.g. float)

TODO: implement and benchmark compression and decompression

Benchmark results
- without compression:
* 2-3 seconds for loading one BIO-ORACLE layer
* 13mb RAM memory usage per BIO-ORACLE layer (original ASCII file: 56MB)
* 5 seconds for retrieving 100.000 random values from 6 Bio-ORACLE layers

Big file benchmark results
- without compression:
* 7-9 minutes for loading one MARSPEC 30seconds layer 
* 1.4 GB ram usage
* 1.5 seconds for retrieving 10.000 random values from 1 MARSPEC layer
* 15 seconds for retrieving 100.000 random values from 1 MARSPEC layer

Marspec 10m benchmark results
- without compression:
* 1 second to load one file
* <4mb RAM memory per MARSPEC 10m layer (original ASCII file: 12MB)
* 50 seconds for retrieving 100.000 random values from 40 layers



## ALTERNATIVES AND IDEAS

- use the research from Daniel Lemire 
    http://lemire.me/blog/archives/2012/09/12/fast-integer-compression-decoding-billions-of-integers-per-second/ 
    http://arxiv.org/abs/1209.2137
    https://github.com/lemire/FastPFor
    https://github.com/lemire/JavaFastPFOR/
- use RASDAMAN http://www.rasdaman.org/

Benchmarks alternative options

- store raster in POSTGIS
* 25seconds to retrieve 1000 values for one MARSPEC 10m layer (stored in the database)
* 20seconds to retrieve 500 values for two MARSPEC 10m layers (stored in the database)
* 29 seconds for 100000 values from 2 MARSPEC 10m layers (stored in database with tiles of size 54x27)
* total RAM usage 15mb

- use protobuf-net https://code.google.com/p/protobuf-net/
    result: slower and memory usage * 3

*)

module BitMap =
    type bitmap = System.Collections.BitArray
    let init (bools:bool []) = new System.Collections.BitArray(bools)

    let isSet index (b:bitmap) =
        b.Get(index)

    let countUpto (uptoIndex:int) (b:bitmap) = 
        let mutable count = 0
        for i=0 to uptoIndex-1 do
            if b.Get(i) then
                count <- count+1
        count

module RasterCompressed =
    open Microsoft.FSharp.Core.Operators
    type Row = int []
//    type Value = int16
//    let convertToDeltaType = Checked.int16
    
    type Value = int32
    let convertToDeltaType = int32
    
    let convertFromDeltaType = int32
    type DeltaRow = { Start:int option; Delta: Value []}
    type RasterInfo = {ColumnCount:int; RowCount:int; ValueScaleFactor:float; Nodata:string}
    
//    type CompressedDelta = MemoryStream
    type CompressedRow = DeltaRow
    //type CompressedRow = {Start:int option; PositiveShift: int; Row: Value []}
//    type CompressedRow = { Length:int; DeltaStart:int option; Row: Value []}
    type RasterData = { Bitmap: BitArray []; Data:(int option*BitArray*CompressedRow) []}
    
    let toDeltaRow (row:Row) =
        if row.Length > 0 then
            let start = row.[0]
            let delta:Value [] = Array.zeroCreate (row.Length-1)
            let mutable current = start
            let mutable previous = start
            for i=0 to row.Length-2 do
                current <- row.[i+1]
                // TODO improve this by e.g. making the type dynamic or doing the type conversion in the compression step
                let mutable diff = current-previous
                if diff > (int System.Int16.MaxValue) || diff < (int System.Int16.MinValue) then
                    printfn "OVERFLOW in delta %d %d" current previous
                    if diff > (int System.Int16.MaxValue) then diff <- (int System.Int16.MaxValue) else diff <- (int System.Int16.MinValue)
                delta.[i] <- convertToDeltaType (diff) // check for overloads
                previous <- current
            { Start=Some(start) ; Delta=delta }
        else
            { Start=None ; Delta=Array.zeroCreate 0 }

    let fromDeltaRow (dr:DeltaRow) =
        if dr.Start.IsSome then
            let row:Value [] = Array.zeroCreate (dr.Delta.Length+1)        
            row.[0] <- dr.Start.Value
            let mutable temp = dr.Start.Value
            for i=1 to dr.Delta.Length do
                temp <- temp + (convertFromDeltaType dr.Delta.[i-1])
                row.[i] <- temp
            row
        else 
            Array.zeroCreate 0

    //FastPFOR gives errors with negative numbers
    let private codec:IntegerCODEC = new Composition( new FastPFOR(), new VariableByte()) :> IntegerCODEC
    let compressFastPFOR (deltaRow:DeltaRow) = 
        if deltaRow.Delta.Length > 0 then
            let inputoffset = new IntWrapper(0)
            let outputoffset = new IntWrapper(0)
            let compressed = Array.zeroCreate deltaRow.Delta.Length
            codec.compress(deltaRow.Delta, inputoffset, deltaRow.Delta.Length, compressed, outputoffset)
        
//        // we can repack the data: (optional)
//        compressed = Arrays.copyOf(compressed,outputoffset.intValue());
            { deltaRow with Delta = compressed }
        else
            deltaRow

    let uncompressFastPFOR info (compressed:CompressedRow) = 
        if compressed.Delta.Length > 0 then
            let recoffset = new IntWrapper(0)
            let recovered = Array.zeroCreate info.ColumnCount
            codec.uncompress(compressed.Delta, new IntWrapper(0), compressed.Delta.Length, recovered, recoffset)
            { compressed with Delta=recovered}
        else
            compressed

    let compress (row:Row) = 
        row 
        |> toDeltaRow 
        //|> compressFastPFOR
    let decompress info (row:CompressedRow) =  
        row 
        //|> uncompressFastPFOR info 
        |> fromDeltaRow

    let parseValue info (v:string) =
        if v <> info.Nodata then
            let scaledValue = info.ValueScaleFactor * (float v)
            Some(int (Math.Round(scaledValue)))
        else
            None

    let parseRow info (values:string []) =
        let parsed = values |> Array.map (parseValue info)
        let bitmap = parsed |> Array.map Option.isSome |> BitMap.init

        let delta = 
            parsed 
            |> Array.choose id
            |> compress

        let (bitmap2, delta2) =
            let parsed = delta.Delta |> Array.map (fun x -> (if x <> 0 then Some(x) else None))
            let bitmap2 = parsed |> Array.map Option.isSome |> BitMap.init
            let delta2 = parsed |> Array.choose id |> compress
            bitmap2, delta2
        (bitmap,(delta.Start, bitmap2,delta2))
    

    let toIndex (v:int<px>) = (int v) - 1

    let isNodata x y data = not (data.Bitmap.[y].Get(x))

//    let getValue info data x y = 
//        let x = toIndex x
//        let y = toIndex y
//        if isNodata x y data then
//            None
//        else
//            let x = BitMap.countUpto x data.Bitmap.[y]
//            let v = 
//                data.Data.[y]
//                |> decompress info // TODO: make this faster by only converting delta's up to the needed index + even faster by storing also the end value and calculating from the back when X is near the end
//                |> (fun arr -> (Array.get arr x))
//                |> float
//                |> (/) <| (info.ValueScaleFactor) // rescale
//            Some(v)

    let getValue info data x y = 
        let x = toIndex x
        let y = toIndex y
        if isNodata x y data then
            None
        else
            let x = BitMap.countUpto x data.Bitmap.[y]
            let deltaStart, bitmap2, delta2 = data.Data.[y]
            let x = BitMap.countUpto x bitmap2
            let toDelta (row:Value[]) = {Start=deltaStart; Delta=row}
            let v = 
                delta2
                |> decompress info
                |> toDelta
                |> decompress info
                |> (fun arr -> (Array.get arr x))
                |> float
                |> (/) <| (info.ValueScaleFactor) // rescale
            Some(v)
    
    let loadAscii path valueScaleFactor =
        let lines = File.ReadLines(path)
        let header = lines.Take(6).ToDictionary((fun (l:string) -> l.Split([|' '|]).[0]), (fun (l:string) -> l.TrimEnd([|'\n'|]).Split([|' '|]).Last()))

        let rowCount = int header.["nrows"]
        let colCount = int header.["ncols"]
        let nodata = header.["NODATA_value"]

        let info = {ColumnCount=colCount; RowCount=rowCount; ValueScaleFactor=valueScaleFactor; Nodata=nodata}
        let splitLine (x:string) = x.Trim([|' '; '\n'|]).Split([|' '|])
        let values = lines.Skip(6)

        let bitmap, deltas = 
            values
            |> Seq.map (splitLine >> (parseRow info))
            |> Array.ofSeq
            |> Array.unzip
        
        let data = { Bitmap= bitmap; Data=deltas}
        (info,data)
    
    let benchmarkLoad p s =
        let memBefore = GC.GetTotalMemory(true) 
        let info, data = OutlierAlgorithms.timeit "loadAscii" (loadAscii p) s
        printfn "Memory usage %d for path %s" ((GC.GetTotalMemory(true) - memBefore) / 1000L) p
        info,data

    let test() = 
        let rows = [|[|1;2;5|];[|3;7;9|]|]
        let delta = rows |> Array.map toDeltaRow
        let undelta = delta |> Array.map fromDeltaRow
        rows = undelta

    let benchmark() =
        let s1, p1 = 1.0, @"D:\a\data\marspec\MARSPEC_30s\ascii\bathy_30s.asc"
        let s2, p2 = 23.0, @"D:\a\data\BioOracle_GLOBAL_RV\sst_min.asc"
        let info1, data1 = benchmarkLoad @"D:\a\data\BioOracle_GLOBAL_RV\sst_min.asc" 1000.0
        let info2, data2 = benchmarkLoad @"D:\a\data\BioOracle_GLOBAL_RV\sst_max.asc" 1000.0
        let info3, data3 = benchmarkLoad @"D:\a\data\BioOracle_GLOBAL_RV\sst_mean.asc" 1000.0
        let info4, data4 = benchmarkLoad @"D:\a\data\BioOracle_GLOBAL_RV\chlo_max.asc" 100.0
        let info5, data5 = benchmarkLoad @"D:\a\data\BioOracle_GLOBAL_RV\chlo_mean.asc" 100.0
        let info6, data6 = benchmarkLoad @"D:\a\data\BioOracle_GLOBAL_RV\chlo_min.asc" 100.0
        let v = (getValue info1 data1 4<px> 1<px>)
        let v2 = getValue info1 data1 2160<px> 1080<px>        

        let r = new System.Random(1)
        let sw = new System.Diagnostics.Stopwatch()
        let len = 1000
        let arr : int64 [] = Array.zeroCreate len
        
        for i=0 to len-1 do
            sw.Restart()
            for j=0 to 100 do
                let x = r.Next(1, 4320) * 1<px>
                let y = r.Next(1, 2160) * 1<px>
                let v2 = getValue info2 data2 x y
                let v3 = getValue info3 data3 x y
                let v4 = getValue info4 data4 x y
                let v5 = getValue info5 data5 x y
                let v6 = getValue info6 data6 x y
                getValue info1 data1 x y
                |> ignore
                
            sw.Stop()
            arr.[i] <- sw.ElapsedMilliseconds
        let filtered = Array.filter (fun x -> x > 1L) arr
        printfn "avg %f" (Array.averageBy float arr)
        printfn "min %d" (Array.min arr)
        printfn "max %d" (Array.max arr)
        printfn "sum %d" (Array.sum arr)

    let benchmarkBigFile() = 
        let s1, p1 = 1.0, @"D:\a\data\marspec\MARSPEC_30s\ascii\bathy_30s.asc"
        let info1, data1 = benchmarkLoad p1 s1

        let r = new System.Random(1)
        let sw = new System.Diagnostics.Stopwatch()
        let len = 1000
        let arr : int64 [] = Array.zeroCreate len
        
        for i=0 to len-1 do
            sw.Restart()
            for j=0 to 100 do
                let x = r.Next(1, 4320) * 1<px>
                let y = r.Next(1, 2160) * 1<px>
                getValue info1 data1 x y
                |> ignore
                
            sw.Stop()
            arr.[i] <- sw.ElapsedMilliseconds
        let filtered = Array.filter (fun x -> x > 1L) arr
        printfn "avg %f" (Array.averageBy float arr)
        printfn "min %d" (Array.min arr)
        printfn "max %d" (Array.max arr)
        printfn "sum %d" (Array.sum arr)
    
    open System.IO
    let benchmark_marspec_10m() =
        let root = @"D:\a\data\marspec\MARSPEC_10m\ascii\"
        let paths = Directory.GetFiles(root, "*.asc")
        // marspec is already scaled
        let load p = benchmarkLoad p 1.0
        
        let marspec = paths |> Array.map load
             

        let r = new System.Random(1)
        let sw = new System.Diagnostics.Stopwatch()
        let len = 1000
        let arr : int64 [] = Array.zeroCreate len
        let info1 = fst (marspec.First())
        let maxX, maxY = info1.ColumnCount, info1.RowCount

        for i=0 to len-1 do
            sw.Restart()
            for j=0 to 100 do
                let x = r.Next(1, maxX) * 1<px>
                let y = r.Next(1, maxY) * 1<px>
                
                Array.map (fun (info, data) -> getValue info data x y) marspec
                |> ignore
                
            sw.Stop()
            arr.[i] <- sw.ElapsedMilliseconds

        printfn "avg %f" (Array.averageBy float arr)
        printfn "min %d" (Array.min arr)
        printfn "max %d" (Array.max arr)
        printfn "sum %d" (Array.sum arr)


// PROTOBUF EXPERIMENT
//    let compress (deltaRow:DeltaRow) = 
//        let stream = new MemoryStream()
//        ProtoBuf.Serializer.Serialize(stream, deltaRow)
//        stream
//
//    let decompress (deltaRow:CompressedDelta) = 
//        deltaRow.Position <- 0L
//        let uncompressedRow = ProtoBuf.Serializer.Deserialize<DeltaRow>(deltaRow)
//        uncompressedRow