module LastContactProxy

open System
open Serilog
open Dtos
open Config
open JsonHttpClient

let getLastContacts () =     
    try
        get<LastContact []>(new Uri(lastContactUrl))
    with 
    | ex -> 
        Log.Error("getLastContacts() - Exception: {ex}", ex)
        [||]