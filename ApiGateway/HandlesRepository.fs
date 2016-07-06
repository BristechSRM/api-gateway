module HandlesRepository

open Config
open Newtonsoft.Json
open System
open System.Net.Http
open System.Net
open Serilog
open Dtos

let getHandlesByProfileId (pid : Guid) = 
    use client = new HttpClient()

    let result = client.GetAsync(handlesUrl + "?profileId=" + pid.ToString()).Result
    match result.StatusCode with
    | HttpStatusCode.OK -> 
        let handlesJson = result.Content.ReadAsStringAsync().Result
        Log.Information("Handles endpoint found")
        let handles = JsonConvert.DeserializeObject<Handle []>(handlesJson)
        handles
    | _ -> 
        let message = sprintf "Error Fetching handles: Status code: %A. Reason: %s" result.StatusCode result.ReasonPhrase
        Log.Error(message)
        raise <| Exception(message)