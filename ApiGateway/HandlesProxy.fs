module HandlesProxy

open Config
open Newtonsoft.Json
open System
open System.Net.Http
open System.Net
open Serilog
open Dtos
open RestModels
open System.Text

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

let getHandle (hid : int) = 
    use client = new HttpClient()
    
    let result = client.GetAsync(handlesUrl + hid.ToString()).Result
    match result.StatusCode with
    | HttpStatusCode.OK -> 
        let handleJson = result.Content.ReadAsStringAsync().Result
        Log.Information("Profiles endpoint found")
        JsonConvert.DeserializeObject<Handle>(handleJson)
    | _ -> 
        let message = sprintf "Error Fetching handle: Status code: %A. Reason: %s" result.StatusCode result.ReasonPhrase
        Log.Error(message)
        raise <| Exception(message)

let patchHandle (hid : int) (op : PatchOp) =
    use client = new HttpClient()

    let data = JsonConvert.SerializeObject(op)
    use content = new StringContent(data, Encoding.UTF8, "application/json")
    use message = new HttpRequestMessage(new HttpMethod("PATCH"), handlesUrl + hid.ToString(), Content = content)
    let result = client.SendAsync(message).Result

    match result.StatusCode with
    | HttpStatusCode.NoContent -> ()
    | _ -> 
        let message = sprintf "Error patching handle: Status code: %A. Reason: %s" result.StatusCode result.ReasonPhrase
        Log.Error(message)
        raise <| Exception(message)