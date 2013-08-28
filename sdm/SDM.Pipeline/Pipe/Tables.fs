namespace SB.SDM.Pipeline.Pipe

open Dapper
open System.Configuration

module DB =
    let connect() = 
        let connString = ConfigurationManager.ConnectionStrings.["DB"].ConnectionString
        let conn = new Npgsql.NpgsqlConnection(connString)
        conn.Open()
        conn

[<CLIMutable>]
type species = {
      
      mutable id       : int64
      mutable tname_id  : int
      mutable coastal_count : int
      mutable total_count : int

      mutable dup_30s : int
      
      mutable dup_oracle : int
}


[<CLIMutable>]
type SpeciesOccurrence = {
    mutable species : string
    mutable longitude : float
    mutable latitude : float
}

[<CLIMutable>]
type OccurrencePixel = {
    mutable species_id : int
    mutable tname_id : int
    mutable pixel_x : int
    mutable pixel_y : int
}

[<CLIMutable>]
type Presence = {
    mutable id : int
    mutable lat : float
    mutable lon : float
    mutable tname_id : int
    mutable resource_id : int
}