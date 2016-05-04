module ProfilesRepository

open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Models

let profilesUri = "http://sessions:8008/profiles/"

let convertToHandle (dto : HandleDto) : Handle =
    { Type = dto.Type
      Identifier = dto.Identifier }

let convertToAdmin (profile : ProfileDto) : Admin =
    { Id = profile.Id
      Forename = profile.Forename
      Surname = profile.Surname
      ImageUri = profile.ImageUrl
      Handles = profile.Handles |> Array.map convertToHandle }

let convertToSpeaker (profile : ProfileDto) : Speaker =
    { Id = profile.Id
      Forename = profile.Forename
      Surname = profile.Surname
      Rating = profile.Rating
      ImageUri = profile.ImageUrl
      Bio = ""
      Handles = profile.Handles |> Array.map convertToHandle }

let getProfile(id : Guid) =
    use client = new HttpClient()
    
    try
        let result = client.GetAsync(profilesUri + id.ToString()).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let sessionJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Profiles endpoint found")
            let profile = JsonConvert.DeserializeObject<ProfileDto>(sessionJson)
            Success(profile)
        | _ ->
            Log.Information("Status code: {statusCode}. Reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase)
            Failure { HttpStatusCode = result.StatusCode; Body = result.ReasonPhrase }
    with
        | :? AggregateException ->
            Log.Information("Could not reach session endpoint")
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "Could not reach sessions endpoint" }
        | ex ->
            Log.Information("Unhandled exception: {message}", ex.Message)
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred: " + ex.Message }

let getAdmin id =
    match getProfile id with
    | Success profile -> convertToAdmin profile |> Success
    | Failure error -> Failure error

let getSpeaker id =
    match getProfile id with
    | Success profile -> convertToSpeaker profile |> Success
    | Failure error -> Failure error
