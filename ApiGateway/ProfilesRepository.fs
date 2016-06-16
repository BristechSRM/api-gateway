module ProfilesRepository

open System
open System.Text
open System.Configuration
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Models

let profilesUri = 
    let url = ConfigurationManager.AppSettings.Get("ProfilesUrl")
    if String.IsNullOrEmpty url then
        failwith "Missing configuration value: 'ProfilesUrl'"
    else
        url

let handleDtoToHandle (dto : HandleDto) : Handle =
    { Type = dto.Type
      Identifier = dto.Identifier }

let handleToHandleDto (handle : Handle) : HandleDto = 
    { Type = handle.Type
      Identifier = handle.Identifier }

let profileToAdmin (profile : ProfileDto) : Admin =
    { Id = profile.Id
      Forename = profile.Forename
      Surname = profile.Surname
      ImageUri = profile.ImageUrl
      Handles = profile.Handles |> Seq.map handleDtoToHandle }

let profileToSpeaker (profile : ProfileDto) : Speaker =
    { Id = profile.Id
      Forename = profile.Forename
      Surname = profile.Surname
      Rating = profile.Rating
      ImageUri = profile.ImageUrl
      Bio = profile.Bio
      Handles = profile.Handles |> Seq.map handleDtoToHandle }

let speakerToProfile (speaker : Speaker) : ProfileDto = 
    { Id = speaker.Id 
      Forename = speaker.Forename
      Surname = speaker.Surname
      Rating = speaker.Rating
      ImageUrl = speaker.ImageUri
      Bio = speaker.Bio
      Handles = speaker.Handles |> Seq.map handleToHandleDto }

let getProfile(pid : Guid) =
    use client = new HttpClient()
    
    try
        let result = client.GetAsync(profilesUri + pid.ToString()).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let sessionJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Profiles endpoint found")
            let profile = JsonConvert.DeserializeObject<ProfileDto>(sessionJson)
            Success(profile)
        | _ ->
            Log.Error("Error Fetching profile: Status code: {statusCode}. Reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase)
            Failure { HttpStatusCode = result.StatusCode; Body = result.ReasonPhrase }
    with
        | ex ->
            Log.Error("Unhandled exception: {message}", ex)
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred:\n" + ex.ToString() }

let updateProfile (pid : Guid) (profile : ProfileDto) = 
    use client = new HttpClient()

    try
        let data = JsonConvert.SerializeObject(profile)
        let content = new StringContent(data,Encoding.UTF8,"application/json")
        let result = client.PutAsync(profilesUri + pid.ToString(),content).Result

        match result.StatusCode with
        | HttpStatusCode.OK -> getProfile pid
        | _ -> 
            Log.Information("Error updating profile Status code: {statusCode}. Reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase)
            Failure { HttpStatusCode = result.StatusCode; Body = result.ReasonPhrase }
    with
        | ex ->
            Log.Error("Unhandled exception: {message}", ex)
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred:\n" + ex.ToString() }

let getAdmin aid =
    match getProfile aid with
    | Success profile -> profileToAdmin profile |> Success
    | Failure error -> Failure error

let getSpeaker sid =
    match getProfile sid with
    | Success profile -> profileToSpeaker profile |> Success
    | Failure error -> Failure error


let updateSpeaker sid (speaker : Speaker) = 
    if sid <> speaker.Id then
        Failure { HttpStatusCode = HttpStatusCode.BadRequest 
                  Body = "Invalid Data. specified speaker Id in request url does not match Id of input speaker"}
    else 
        let profile = speakerToProfile speaker
        match updateProfile sid profile with
        | Success profile -> profileToSpeaker profile |> Success
        | Failure error -> Failure error
