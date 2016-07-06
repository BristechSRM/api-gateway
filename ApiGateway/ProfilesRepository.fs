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
open DataTransform

let profilesUri = 
    let url = ConfigurationManager.AppSettings.Get("ProfilesUrl")
    if String.IsNullOrEmpty url then
        failwith "Missing configuration value: 'ProfilesUrl'"
    else
        url

let handleToHandleDto (handle : Models.Handle) : Dtos.HandleDto = 
    { Type = handle.Type
      Identifier = handle.Identifier }

let speakerToProfile (speaker : Speaker) : Profile = 
    { Id = speaker.Id 
      Forename = speaker.Forename
      Surname = speaker.Surname
      Rating = speaker.Rating
      ImageUrl = speaker.ImageUri
      Bio = speaker.Bio }
      //Handles = speaker.Handles |> Seq.map handleToHandleDto }

let getProfile(pid : Guid) =
    use client = new HttpClient()
    
    let result = client.GetAsync(profilesUri + pid.ToString()).Result
    match result.StatusCode with
    | HttpStatusCode.OK ->
        let sessionJson = result.Content.ReadAsStringAsync().Result
        Log.Information("Profiles endpoint found")
        let profile = JsonConvert.DeserializeObject<Profile>(sessionJson)
        profile
    | _ ->
        let message = sprintf "Error Fetching profile: Status code: %A. Reason: %s" result.StatusCode result.ReasonPhrase
        Log.Error(message)
        raise <| Exception(message)

//Currently broken. Handles aren't dealt with since sessions change. 
let updateProfile (pid : Guid) (profile : Profile) = 
    use client = new HttpClient()

    try
        let data = JsonConvert.SerializeObject(profile)
        let content = new StringContent(data,Encoding.UTF8,"application/json")
        let result = client.PutAsync(profilesUri + pid.ToString(),content).Result

        match result.StatusCode with
        | HttpStatusCode.OK -> Success <| getProfile pid
        | _ -> 
            Log.Information("Error updating profile Status code: {statusCode}. Reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase)
            Failure { HttpStatusCode = result.StatusCode; Body = result.ReasonPhrase }
    with
        | ex ->
            Log.Error("Unhandled exception: {message}", ex)
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred:\n" + ex.ToString() }

let updateSpeaker sid (speaker : Speaker) = 
    if sid <> speaker.Id then
        Failure { HttpStatusCode = HttpStatusCode.BadRequest 
                  Body = "Invalid Data. specified speaker Id in request url does not match Id of input speaker"}
    else 
        let profile = speakerToProfile speaker
        match updateProfile sid profile with
        | Success profile -> Profile.toSpeaker [] profile |> Success //TODO handles
        | Failure error -> Failure error
