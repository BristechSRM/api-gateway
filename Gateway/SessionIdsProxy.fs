module SessionIdsProxy

open Config
open JsonHttpClient
open System

let getSessionIdsByEventId (eventId : Guid) = get<Guid []> <| new Uri(sessionIdsUri, "?eventId=" + eventId.ToString())
