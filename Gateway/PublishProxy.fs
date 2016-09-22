module PublishProxy

open Config
open JsonHttpClient
open System
open Models

let publishEvent (me : MeetupEvent) = postAndGetGuid (new Uri(publishUri, "?eventId=" + me.EventId.ToString())) ""

let deleteEvent (meId : Guid) = delete (new Uri(publishUri, "?meetupEventId=" + meId.ToString()))
