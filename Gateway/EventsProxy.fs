module EventsProxy

open Config
open Dtos
open JsonHttpClient
open System

let getEvents() =
  get<Event []> eventsUri
