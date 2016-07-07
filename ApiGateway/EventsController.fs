namespace Controllers

open System.Net.Http
open System.Web.Http
open Serilog
open EventsProxy
open Models
open System

type EventsController() =
    inherit ApiController()

    member x.Get() =
        Log.Information("Received GET request for events")
        match getEvents() with
        | Success events -> x.Request.CreateResponse(events)
        | Failure error -> x.Request.CreateResponse(error.HttpStatusCode, error.Body)

    member x.Get(id : string) =
        Log.Information("Received GET request for event with id {id}", id)
        match getEvent(id) with
        | Success event -> x.Request.CreateResponse(event)
        | Failure error -> x.Request.CreateResponse(error.HttpStatusCode, error.Body)