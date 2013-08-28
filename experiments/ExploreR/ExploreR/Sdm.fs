namespace ExploreR

open RProvider
open RProvider.``base``

open FSharp.Data

open System
open System.Net

module Sdm =


    let v = R.c(1,2,3)
    // RProvider.biomod2.R.

module Query =
    let create prefix value = sprintf "q=%s\"%s\"" prefix value
    let all = create ""
    let author = create "author:"
    let title = create "title:"
    let subject = create "subject:"
    let ``abstract`` = create "abstract:"
    let compose q1 q2 = String.Join("&", q1, q2)
    let join (q: string seq) = String.Join("%", Array.ofSeq(q))


type Extra = 
    { apikey: string; start: int option; rows: int option; json: bool option; fields: string [] option; sort: string}

    member private this.add (prefix:string) q = prefix + q.ToString()
    member private this.t q action =
        match q with 
        | Some(x) -> action(x)
        | None -> ""

    member this.queryArgs() =
        let s = this.t this.start (this.add "start=")
        let fl = if this.fields.IsNone then "" else "fl=" + String.Join(",", Some(this.fields))
        let fq = "fq=!article_type_facet:\"Issue Image\" AND doc_type:full" // full documents only
        printf "api_key=%s&%s&%s" this.apikey fl fq

//type Extr(apikey:string, ?fields:string [], ?start:int, ?rows: int, ?json:bool, ?sort:string) =
//    
//    
    
    //sprintf "api_key=%s" apikey


module Plos =
    let buildQuery (q: string, extra: Extra) =

        let b = "http://api.plos.org/search/?"
        
        ""


type Class1() = 
    member this.X = "F#"
