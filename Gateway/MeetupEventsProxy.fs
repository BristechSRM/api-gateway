module MeetupEventsProxy

open Config
open Dtos
open JsonHttpClient
open System

let getMeetupEvent(id : Guid) = get<MeetupEvent> <| new Uri(meetupEventsUri, id.ToString())

let postMeetupEvent(meetup: MeetupEvent) = postAndGetGuid meetupEventsUri meetup
