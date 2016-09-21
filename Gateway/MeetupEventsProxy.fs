module MeetupEventsProxy

open Config
open Dtos
open JsonHttpClient
open System
open DataTransform

let getMeetupEvent(id : Guid) = get<MeetupEvent> <| new Uri(meetupEventsUri, id.ToString()) |> MeetupEvent.toModel

let postMeetupEvent(meetup: MeetupEvent) = postAndGetGuid meetupEventsUri meetup
