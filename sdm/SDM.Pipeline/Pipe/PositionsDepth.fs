namespace SB.SDM.Pipeline.Pipe

open System.IO
open System
open System.Globalization
open System.Configuration
open Dapper
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
        //let height = width / 2
        let x = toPixel 360.0 info.ColumnCount position.Lon
        let y = toPixel -180.0 info.RowCount position.Lat // -180.0 because we count in the opposite direction
        1

    let toPosition (row:String) = 
        let columns = row.Split('|')
        { Id= (int (columns.[0])); Lon=(float (columns.[1])); Lat=(float (columns.[2])) }

    let loadPositions file = 
        File.ReadAllLines file
        |> Seq.map toPosition



    let positions = 
        //let conn = DB.connect()
        //let positions = conn.Query<Position>("SELECT id, longitude lon, latitude lat FROM obis.positions ORDER BY longitude, latitude")

        let positions = loadPositions """D:\temp\all_positions.csv"""

        positions


        // calculate raster x, y
        // get value from the 3 rasters
        // update value in DB
        let rasters = ["""gebco_gridone.asc""";"""etopo1_bed_c.asc"""; """bathy_30s.asc"""]
                      |> Seq.map (fun p -> RasterCompressed.loadAscii p 1.0)
        
        
        //getValue info data x y

        1
        //let marspecIdXy = Seq.map (toIdxy info.ColumnCount) positions 

//        let trans = conn.BeginTransaction()
//        let depth = 2.0
//        let ids = [|1|]
//        let idstr = String.Join(",", (Seq.map string ids))
//        //let updateCount = conn.Execute((sprintf "UPDATE obis.positions SET mindepth = %f WHERE id IN (%s)" depth idstr) , transaction=trans)
//
//        trans.Commit()

        
         
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