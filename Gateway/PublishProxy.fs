module PublishProxy

open Config
open JsonHttpClient
open System

let publishEvent (eventId : Guid) = postAndGetGuid (new Uri(publishUri, "?eventId=" + eventId.ToString())) ""
