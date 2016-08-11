module EventsProxy

open Config
open Dtos
open JsonHttpClient
open System

let getEvents() =
  get<Event []> eventsUri

let getEvent(id : Guid) =
  get<Event> <| new Uri(eventsUri, id.ToString())

let postEvent(event: Event) =
  postAndGetGuid eventsUri event
