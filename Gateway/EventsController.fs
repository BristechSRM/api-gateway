module EventsController

open System
open EventsFacade
open DataTransform
open Models
open System.Net
open System.Net.Http
open System.Web.Http

type EventsController() =
    inherit ApiController()

    member x.Get() = (fun () -> getEventSummaries()) |> Catch.respond x HttpStatusCode.OK

    member x.Get (id : Guid) = (fun () -> getEventDetail id) |> Catch.respond x HttpStatusCode.OK    

    member this.Post(event: Event) =
        let guid = event |> Event.toDto |> EventsProxy.postEvent
        this.Request.CreateResponse(HttpStatusCode.Created, guid)

    [<Route("events/publish")>]
    [<HttpPost>]
    member this.PublishEvent(eventId : Guid) = 
        let result = PublishProxy.publishEvent eventId
        this.Request.CreateResponse(HttpStatusCode.NoContent)
