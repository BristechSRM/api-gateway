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
open JsonHttpClient

let getHandlesByProfileId (pid : Guid) = get<Handle []>(new Uri(handlesUrl + "?profileId=" + pid.ToString()))

let getHandle (hid : int) = get<Handle>(new Uri(handlesUrl + hid.ToString()))

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