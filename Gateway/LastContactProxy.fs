module LastContactProxy

open System
open Serilog
open Dtos
open Config
open JsonHttpClient

let getLastContacts () =     
    try
        get<LastContact []> lastContactUri
    with 
    | ex -> 
        Log.Error("getLastContacts() - Exception: {ex}", ex)
        [||]
