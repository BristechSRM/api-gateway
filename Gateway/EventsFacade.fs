module EventsFacade

open System
open Dtos
open DataTransform
open SpeakerFacade
open SessionsProxy
open SessionIdsProxy  

let getEventSummary (eventId : Guid) = 
    let event = EventsProxy.getEvent eventId
    let sessionIds = getSessionIdsByEventId eventId
    Event.toSummary sessionIds event

let getEventSummaries() = 
    EventsProxy.getEvents()
    |> Array.map (fun event -> 
        let sessionIds = getSessionIdsByEventId event.Id
        Event.toSummary sessionIds event)

let getEventDetail (eventId: Guid) = 
    let record = EventsProxy.getEvent eventId
    let eventSessions = 
        getSessionsByEventId eventId 
        |> Array.map (fun session -> 
            let speaker = getSpeaker session.SpeakerId
            Session.toEventSession speaker session)
    Event.toDetail eventSessions record
