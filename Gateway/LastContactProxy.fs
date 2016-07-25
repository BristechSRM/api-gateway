module LastContactProxy

open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Config

let getLastContacts() =
    use client = new HttpClient()

    try
        let lastContactJson = client.GetAsync(lastContactUrl).Result.Content.ReadAsStringAsync().Result
        Log.Information("Last contact endpoint found")
        JsonConvert.DeserializeObject<LastContact[]>(lastContactJson)
    with
    | ex ->
        Log.Error("getLastContacts() - Exception: {ex}", ex)
        [||]