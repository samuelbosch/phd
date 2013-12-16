namespace OBIS.QC.Test

open NUnit.Framework
open FsUnit
open OBIS.QC.Core
open ObisDb
open System
open System.Configuration
open System.Data
open GeoAPI.Geometries
open NetTopologySuite.Geometries

[<TestFixture>]
type ``Given LandDistance module``() = 
    
    [<Test>] 
    member x.``When get distance non intersecting then correct value returned`` () = 
        LandDistance.run [{Id=1;Lon=(-84.8600006103516);Lat=9.82999992370605}] 
        //LandDistance.run [{Id=1;Lon=0.0;Lat=0.0}] 
        |> List.head
        |> fun d -> truncate d.LandDistance
        |> should equal 269

    [<Test>] 
    member x.``When get distance intersecting then correct value returned`` () = 
        LandDistance.run [{Id=1;Lon=(-78.5);Lat=8.47000026702881}] 
        |> List.head
        |> fun d -> truncate d.LandDistance
        |> should equal -655

    [<Test>] 
    member x.``When get distance incorrect lat/lon then nothing returned`` () = 
        LandDistance.run [{Id=1;Lon=(-78.5);Lat=90.1};{Id=1;Lon=(-180.1);Lat=89.0}] 
        |> should equal List.empty

    [<Test>] 
    member x.``test performance improvements`` () = 
        let sw = System.Diagnostics.Stopwatch.StartNew()
        
        let trans = LandDistance.buildTransformer (new Point(-84.8600006103516, 9.82999992370605))

        for t in seq { 0 .. 24 } do
            seq { -89.0 .. 0.001 .. 89.0 }
            |> Seq.iter (fun x -> (trans (new Coordinate(x, x))) |> ignore)
            |> ignore
        sw.Stop()
        printfn "%i" (sw.ElapsedMilliseconds)
        sw.ElapsedMilliseconds |> should lessThan 4000


