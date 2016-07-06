module ProfilesRepository

open Config
open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos

let getProfile(pid : Guid) =
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
