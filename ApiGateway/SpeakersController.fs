namespace Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open Serilog
open HandlesRepository
open ProfilesRepository
open System
open Models
open DataTransform

type SpeakersController() =
    inherit ApiController()

    member x.Get(id : Guid) =
        (fun () -> 
            let profile = getProfile id 
            let handlesDtos = getHandlesByProfileId id 
            Profile.toSpeaker handlesDtos profile) 
        |> Catch.respond x HttpStatusCode.OK 

    member x.Put(id : Guid, updatedSpeaker : Speaker) = 
        Log.Information("Received Put request for speaker with id: {id}", id)
        match updateSpeaker id updatedSpeaker with
        | Success speaker -> 
            Log.Information("Success: Put request for speaker with id: {id} succeeded", id)
            x.Request.CreateResponse(speaker)
        | Failure error -> 
            Log.Error("Put failed with StatusCode: {code} and message {body}",error.HttpStatusCode, error.Body)
            x.Request.CreateErrorResponse(error.HttpStatusCode, error.Body)