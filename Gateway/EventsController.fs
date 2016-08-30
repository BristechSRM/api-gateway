module EventsController

open EventsFacade
open DataTransform
open Models
open System.Net
open System.Net.Http
open System.Web.Http

type EventsController() =
    inherit ApiController()

    member x.Get() = (fun () -> getEventSummaries()) |> Catch.respond x HttpStatusCode.OK

  // TODO: swtich id to guid when event by session date is no longer used. 
    member x.Get (id : string) = 
        match getEventDetail id with  
        | Some event -> x.Request.CreateResponse(HttpStatusCode.OK, event)
        | None -> x.Request.CreateErrorResponse(HttpStatusCode.NotFound, "")            

    member this.Post(event: Event) =
        let guid = event |> Event.toDto |> EventsProxy.postEvent
        this.Request.CreateResponse(HttpStatusCode.Created, guid)
