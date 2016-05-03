namespace Controllers

open System.Net.Http
open System.Web.Http
open Serilog
open ProfilesRepository
open System
open Models

type SpeakersController() =
    inherit ApiController()

    member x.Get(id : Guid) =
        Log.Information("Received GET request for a speaker with id {id}", id)
        match getSpeaker id with
        | Success speaker -> x.Request.CreateResponse(speaker)
        | Failure error -> x.Request.CreateResponse(error.HttpStatusCode, error.Body)