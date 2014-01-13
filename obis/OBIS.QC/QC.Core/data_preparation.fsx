(*
#I @"D:\a\Google Drive\code\obis\OBIS.QC\QC.Core\lib"
*)

#r "lib/Dapper.dll"

open Dapper

let connstr = "Server=127.0.0.1;Port=5439;Database=obis20130703;User Id=postgres;Password=;"
let gebco = @"D:\a\data\gebco\gebco_gridone.asc"
let etopo = @"D:\a\data\etopo\etopo1_ice_c.asc"
let marspec_bathy = @"D:\a\data\marspec\MARSPEC_30s\ascii\bathy_30s.asc"
let gshhs = @"D:\a\data\coastline\GSHHS_i_L1.shp"

