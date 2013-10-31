// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.
// Define your library scripting code here

#I @"D:\a\Google Drive\code\sdm\SDM.Pipeline\Pipe\bin\Release"
#r "Pipe"
open SB.SDM.Pipeline.Pipe
open PositionsDepth
#time "on"


let (info, data) = List.head rasters;;
val p : Position list = [{Id = 13369121;
                          Lon = -122.5216904;
                          Lat = 36.58480835;}]

RasterCompressed.getValue info data (3449*1<px>) (3206*1<px>);;

let r = Raster.loadAscii """D:\a\data\gebco\gebco_gridone.asc""";;
r.Rows |> Seq.skip 3205*21601 |> Seq.skip 3448 |> Seq.head;;

toMinValues [(1,Some(213.0));(1,Some(124.0))];;