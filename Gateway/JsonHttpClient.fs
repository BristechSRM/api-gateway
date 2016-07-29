module JsonHttpClient

open Newtonsoft.Json
open System
open System.Net
open System.Net.Http
open Serilog

let get<'Model> (uri : Uri) = 
    use client = new HttpClient()
    let response = client.GetAsync(uri).Result
    let modelName = typeof<'Model>.Name
    match response.StatusCode with
    | HttpStatusCode.OK -> 
        let json = response.Content.ReadAsStringAsync().Result
        Log.Information("Endpoint for " + modelName + " found.")
        JsonConvert.DeserializeObject<'Model>(json)
    | errorCode -> 
        let errorResponse = response.Content.ReadAsStringAsync().Result
        let message = sprintf "Error in get request for %s. Status code: %i. Reason phrase: %s. Error Message: %s" modelName (int (errorCode)) response.ReasonPhrase errorResponse
        Log.Error(message)
        failwith message
