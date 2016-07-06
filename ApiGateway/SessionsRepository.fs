module SessionsRepository

open Config
open Bristech.Srm.HttpConfig
open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Models
open LastContactRepository
open DataTransform

let getSessions() =  
    use client = new HttpClient()
    let result = client.GetAsync(sessionsUrl).Result
    match result.StatusCode with
    | HttpStatusCode.OK ->
        let sessionJson = result.Content.ReadAsStringAsync().Result
        Log.Information("Sessions endpoint found")
        let sessions = JsonConvert.DeserializeObject<Dtos.Session[]>(sessionJson)
        let lastContacts = getLastContacts()
        sessions |> Seq.map (fun session -> Session.toModel(lastContacts, session))
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
        let session = JsonConvert.DeserializeObject<Dtos.Session>(sessionJson)
        let lastContacts = getLastContacts()
        Session.toModel(lastContacts, session)
    | _ ->
        let message = sprintf "Error Fetching session: Status code: %A. Reason: %s" result.StatusCode result.ReasonPhrase
        Log.Error(message)
        raise <| Exception(message)