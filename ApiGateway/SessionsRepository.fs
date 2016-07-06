module SessionsRepository

open Config
open Bristech.Srm.HttpConfig
open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog

let getSessions() =  
    use client = new HttpClient()
    let result = client.GetAsync(sessionsUrl).Result
    match result.StatusCode with
    | HttpStatusCode.OK ->
        let sessionJson = result.Content.ReadAsStringAsync().Result
        Log.Information("Sessions endpoint found")
        JsonConvert.DeserializeObject<Dtos.Session[]>(sessionJson)
    | _ ->
        let message = sprintf "Error Fetching sessions: Status code: %A. Reason: %s" result.StatusCode result.ReasonPhrase
        Log.Error(message)
        raise <| Exception(message)

let getSession(id : Guid) =
    use client = new HttpClient()
    
    let result = client.GetAsync(sessionsUrl + id.ToString()).Result
    match result.StatusCode with
    | HttpStatusCode.OK ->
        let sessionJson = result.Content.ReadAsStringAsync().Result
        Log.Information("Session endpoint found")
        JsonConvert.DeserializeObject<Dtos.Session>(sessionJson)
    | _ ->
        let message = sprintf "Error Fetching session: Status code: %A. Reason: %s" result.StatusCode result.ReasonPhrase
        Log.Error(message)
        raise <| Exception(message)