module EventsController

open DataTransform
open Models
open System
open System.Net
open System.Net.Http
open System.Web.Http

type EventsController() =
  inherit ApiController()

  // Gets an array of EventSummary
  // TODO: remove facade
  member this.Get() =
    let events = EventsProxy.getEvents()
    let oldevents = EventsFacade.getEvents()
    let allevents = Array.concat [ events |> Seq.map Event.toEventSummary |> Seq.toArray ; oldevents ]
    this.Request.CreateResponse(HttpStatusCode.OK, allevents)

  // Gets a single EventDetail
  // TODO: id: Guid
  // TODO: remove facade
  member this.Get(id: string) =
    try
      let event = EventsProxy.getEvent <| new Guid(id)
      match box event with
      | null ->
        this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "")
      | _ ->
        this.Request.CreateResponse(HttpStatusCode.OK, event |> Event.toEventDetail)
    with
    | ex ->
      let oldevent = EventsFacade.getEvent id
      match box oldevent with
      | null ->
        this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "")
      | _ ->
        this.Request.CreateResponse(HttpStatusCode.OK, oldevent)
