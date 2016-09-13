module PublishProxy

open Config
open JsonHttpClient
open System

let publishEvent (eventId : Guid) = post (new Uri(publishUri, "?eventId=" + eventId.ToString())) ""
