module SessionIdsProxy

open Config
open Dtos
open JsonHttpClient
open RestModels
open System

let getSessionIdsByEventId (eventId : Guid) = get<Guid []> <| new Uri(sessionIdsUri, "?eventId=" + eventId.ToString())