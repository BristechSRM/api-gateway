module EventsFacade

open System
open Dtos
open DataTransform
open SpeakerFacade
open SessionsProxy
open SessionIdsProxy  
open MeetupEventsProxy
open EventsProxy

let getEventSummary (eventId : Guid) = 
    let event = getEvent eventId
    let sessionIds = getSessionIdsByEventId eventId
    let meetupEvent = event.MeetupEventId |> Option.map getMeetupEvent 
    Event.toSummary sessionIds meetupEvent event

let getEventSummaries() = 
    getEvents()
    |> Array.map (fun event -> 
        let sessionIds = getSessionIdsByEventId event.Id
        let meetupEvent = event.MeetupEventId |> Option.map getMeetupEvent
        Event.toSummary sessionIds meetupEvent event)

let getEventDetail (eventId: Guid) = 
    let record = getEvent eventId
    let eventSessions = 
        getSessionsByEventId eventId 
        |> Array.map (fun session -> 
            let speaker = getSpeaker session.SpeakerId
            Session.toEventSession speaker session)
    Event.toDetail eventSessions record
