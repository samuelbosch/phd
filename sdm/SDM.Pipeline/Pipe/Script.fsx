// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.
// Define your library scripting code here
#r @"D:\a\Google Drive\code\sdm\SDM.Pipeline\packages\NetTopologySuite.1.13.1\lib\net40-client\NetTopologySuite.dll"
#r @"D:\a\Google Drive\code\sdm\SDM.Pipeline\packages\NetTopologySuite.IO.1.13.1\lib\net40-client\NetTopologySuite.IO.GeoTools.dll"
#r @"D:\a\Google Drive\code\sdm\SDM.Pipeline\packages\GeoAPI.1.7.1.1\lib\net40-client\GeoAPI.dll"
#r @"D:\a\Google Drive\code\sdm\SDM.Pipeline\packages\ProjNET4GeoAPI.1.3.0.2\lib\net40-client\ProjNet.dll"
#r @"D:\a\Google Drive\code\sdm\SDM.Pipeline\packages\DotSpatial.Projections.1.5.1\lib\net40-Client\DotSpatial.Projections.dll"
#I @"D:\a\Google Drive\code\sdm\SDM.Pipeline\Pipe\bin\Release"


#r "Pipe"
open SB.SDM.Pipeline.Pipe
open PositionsDepth
#time "on"


let (info, data) = List.head rasters;;
let p : Position list = [{Id = 13369121;
                          Lon = -122.5216904;
                          Lat = 36.58480835;}]

RasterCompressed.getValue info data (3449*1<px>) (3206*1<px>);;

let r = Raster.loadAscii """D:\a\data\gebco\gebco_gridone.asc""";;
r.Rows |> Seq.skip 3205*21601 |> Seq.skip 3448 |> Seq.head;;

toMinValues [(1,Some(213.0));(1,Some(124.0))];;