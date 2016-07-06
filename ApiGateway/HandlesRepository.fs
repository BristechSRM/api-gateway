module HandlesRepository

open Newtonsoft.Json
open System
open System.Configuration
open System.Net.Http
open System.Net
open Serilog
open Dtos


//TODO next pull request, refactor uri code to config. 
let handlesUri = 
    let url = ConfigurationManager.AppSettings.Get("HandlesUrl")
    if String.IsNullOrEmpty url then
        failwith "Missing configuration value: 'HandlesUrl'"
    else
        url  

let getHandlesByProfileId (pid : Guid) = 
    use client = new HttpClient()

    let result = client.GetAsync(handlesUri + "?profileId=" + pid.ToString()).Result
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