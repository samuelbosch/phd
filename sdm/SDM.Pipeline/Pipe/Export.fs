namespace SB.SDM.Pipeline.Pipe

open System.IO
open System
open System.Globalization
open System.Configuration
open Dapper

module Export =

    let toCsvLine record =
        String.Join(",", record.species, record.longitude.ToString(CultureInfo.InvariantCulture), record.latitude.ToString(CultureInfo.InvariantCulture))


    let toCsv pathprefix (records:seq<SpeciesOccurrence>) = 
        let path = pathprefix + ( Seq.head records).species + ".csv"
        let lines = Seq.map toCsvLine records
        if File.Exists(path) 
        //then invalidArg "path" (sprintf "Path already exists [%s]." path)
        then
            File.AppendAllLines(path, lines)
            printf "Exported again to %s\n" path
        else 
            let header = "species,longitude,latitude"
            File.WriteAllLines(path, seq{ yield header; yield! lines})
            printfn "Exported to %s" path

    let allSpeciesToCsv pathprefix =
        let conn = DB.connect()

//         let s = conn.Query<species>("""
//SELECT sp.* FROM p.species sp
//LIMIT 1""")
         
        let randomspecies = conn.Query<int>("""
SELECT s.tname_id FROM 
(SELECT s.tname_id, random() FROM p.species s
ORDER BY random()) s
WHERE EXISTS (SELECT tname_id FROM p.presences WHERE tname_id = s.tname_id LIMIT 1)
         """)
        
        let querySpeciesOccurrence (tnameId:int) = conn.Query<SpeciesOccurrence>((sprintf """
SELECT t.tname species, p.lon longitude, p.lat latitude 
FROM p.presences p
JOIN obis.tnames t ON p.tname_id = t.id
WHERE p.tname_id = %d""" tnameId))

        //let test = querySpeciesOccurrence 463332

        randomspecies 
        |> Seq.map querySpeciesOccurrence 
        |> Seq.map (toCsv pathprefix)
        |> Seq.length // force execution of sequence
        |> printfn "Exported %d species" 

    let printduplicates =
        let conn = DB.connect()

        let randomspecies = conn.Query<int>("""
SELECT s.tname_id FROM 
(SELECT s.tname_id, random() FROM p.species s
ORDER BY random()) s
WHERE EXISTS (SELECT tname_id FROM p.presences WHERE tname_id = s.tname_id LIMIT 1)
         """)
        
        let querySpeciesOccurrence (tnameId:int) = conn.Query<SpeciesOccurrence>((sprintf """
SELECT t.tname species, p.lon longitude, p.lat latitude 
FROM p.presences p
JOIN obis.tnames t ON p.tname_id = t.id
WHERE p.tname_id = %d LIMIT 1""" tnameId))
        
        

        randomspecies
        |> Seq.map querySpeciesOccurrence
        |> Seq.map (fun r -> (Seq.head r))