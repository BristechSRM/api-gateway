module ProfilesProxy

open Config
open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open RestModels
open System.Text

let getProfile (pid : Guid) =
    use client = new HttpClient()
    
    let result = client.GetAsync(profilesUrl + pid.ToString()).Result
    match result.StatusCode with
    | HttpStatusCode.OK ->
        let profileJson = result.Content.ReadAsStringAsync().Result
        Log.Information("Profiles endpoint found")
        let profile = JsonConvert.DeserializeObject<Profile>(profileJson)
        profile
    | _ ->
        let message = sprintf "Error Fetching profile: Status code: %A. Reason: %s" result.StatusCode result.ReasonPhrase
        Log.Error(message)
        raise <| Exception(message)

let patchProfile (pid : Guid) (op : PatchOp) = 
    use client = new HttpClient()
    
    let data = JsonConvert.SerializeObject(op)
    use content = new StringContent(data, Encoding.UTF8, "application/json")
    use message = new HttpRequestMessage(new HttpMethod("PATCH"), profilesUrl + pid.ToString(), Content = content)
    let result = client.SendAsync(message).Result

    match result.StatusCode with
    | HttpStatusCode.NoContent -> ()
    | _ -> 
        let message = sprintf "Error Patching profile: Status code: %A. Reason: %s" result.StatusCode result.ReasonPhrase
        Log.Error(message)
        raise <| Exception(message)