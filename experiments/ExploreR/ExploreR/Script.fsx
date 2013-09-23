// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

//#load "Library1.fs"
//open ExploreR

// Define your library scripting code here

#if INTERACTIVE
#r """D:\a\Google Drive\code\experiments\ExploreR\packages\RProvider.1.0.0\lib\RDotNet.dll"""
#r """D:\a\Google Drive\code\experiments\ExploreR\packages\RProvider.1.0.0\lib\RDotNet.NativeLibrary.dll"""
#r "D:\\a\\Google Drive\\code\\experiments\\ExploreR\\packages\\RProvider.1.0.0\\lib\\RProvider.dll"

#r """D:\a\Google Drive\code\experiments\ExploreR\packages\FSharp.Data.1.1.5\lib\net40\FSharp.Data.dll"""
#r """D:\a\Google Drive\code\experiments\ExploreR\packages\FSharp.Data.1.1.5\lib\net40\FSharp.Data.DesignTime.dll"""

#endif
open FSharp.Data
open System
open System.Net
open RProvider
open RProvider.``base``


let v = R.c(1,2,3)
R.require("RPostgreSQL")
let drv = RProvider.DBI.R.dbDriver("PostgreSQL")
//let d = new System.Collections.Generic.Dictionary<string,Object>()
//d.Add("dbname","obis20130703")
let conn = RProvider.DBI.R.dbConnect(drv, [|"obis20130703", 5439, "postgres", ""  |]//port=5439, user="postgres", password="")


