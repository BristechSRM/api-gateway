module SessionsProxy

open Config
open Dtos
open Bristech.Srm.HttpConfig
open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open RestModels
open System.Text
open JsonHttpClient

let getSessions() = get<Session []>(new Uri(sessionsUrl))

let getSession (sid : Guid) = get<Session>(new Uri(sessionsUrl + sid.ToString()))

let addSession (session : Session) = postAndGetGuid (new Uri(sessionsUrl)) session

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
