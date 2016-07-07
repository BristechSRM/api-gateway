module SessionsProxy

open Config
open Bristech.Srm.HttpConfig
open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open RestModels
open System.Text

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

let patchSession (pid : Guid) (op : PatchOp) = 
    use client = new HttpClient()
    
    let data = JsonConvert.SerializeObject(op)
    use content = new StringContent(data, Encoding.UTF8, "application/json")
    use message = new HttpRequestMessage(new HttpMethod("PATCH"), sessionsUrl + pid.ToString(), Content = content)
    let result = client.SendAsync(message).Result

    match result.StatusCode with
    | HttpStatusCode.NoContent -> ()
    | _ -> 
        let message = sprintf "Error Patching session: Status code: %A. Reason: %s" result.StatusCode result.ReasonPhrase
        Log.Error(message)
        raise <| Exception(message)