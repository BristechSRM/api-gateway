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

    member x.Get (id : Guid) = 
        match getEventDetail id with  
        | Some event -> x.Request.CreateResponse(HttpStatusCode.OK, event)
        | None -> x.Request.CreateErrorResponse(HttpStatusCode.NotFound, "")            

    member this.Post(event: Event) =
        let guid = event |> Event.toDto |> EventsProxy.postEvent
        this.Request.CreateResponse(HttpStatusCode.Created, guid)
