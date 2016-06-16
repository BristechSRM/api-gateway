namespace Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open Serilog
open ProfilesRepository
open System
open Models

type SpeakersController() =
    inherit ApiController()

    member x.Get(id : Guid) =
        Log.Information("Received GET request for a speaker with id: {id}", id)
        match getSpeaker id with
        | Success speaker -> x.Request.CreateResponse(speaker)
        | Failure error -> x.Request.CreateResponse(error.HttpStatusCode, error.Body)

    member x.Put(id : Guid, updatedSpeaker : Speaker) = 
        Log.Information("Received Put request for speaker with id: {id}", id)
        match updateSpeaker id updatedSpeaker with
        | Success speaker -> 
            Log.Information("Success: Put request for speaker with id: {id} succeeded", id)
            x.Request.CreateResponse(speaker)
        | Failure error -> 
            Log.Error("Put failed with StatusCode: {code} and message {body}",error.HttpStatusCode, error.Body)
            x.Request.CreateErrorResponse(error.HttpStatusCode, error.Body)