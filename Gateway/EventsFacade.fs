module EventsFacade

open System
open Dtos
open Models
open DataTransform
open SpeakerFacade
open SessionsProxy
open SessionIdsProxy

let convertToISO8601 (datetime : DateTime) = datetime.ToString("yyyy-MM-ddTHH\:mm\:ss\Z")

let getIds (sessions: Dtos.Session[]) = sessions |> Array.map (fun session -> session.Id)

let onSameDay (datetime1: DateTime) (datetime2: DateTime) = datetime1.Date = datetime2.Date

let getEventSummariesByDatedSessions() = 
    getSessions()
    |> Array.filter (fun session -> session.Date.IsSome)
    |> Array.groupBy (fun session -> session.Date.Value)
    |> Array.map (fun (date, eventSessions) -> {EventSummary.Id = date.Date |> convertToISO8601; Date = date; Description = ""; Location = ""; SessionIds = getIds eventSessions})      

let getEventDetailByDatedSessionsAndId(id) = 
    let date = DateTime.Parse(id, null, System.Globalization.DateTimeStyles.RoundtripKind)
    let eventSessions = 
        getSessions()
        |> Array.filter (fun session -> session.Date.IsSome && onSameDay session.Date.Value date)
        |> Array.map (fun session -> 
            let speaker = getSpeaker session.SpeakerId
            Session.toEventSession speaker session)
    let event = { EventDetail.Id = date.Date |> convertToISO8601; Date = date; Description = ""; Location = ""; Sessions = eventSessions }
    event

let getEventSummaries() = 
    let eventsBySessionDates = getEventSummariesByDatedSessions() //TODO remove old event by session date
    let events = 
        EventsProxy.getEvents()
        |> Array.map (fun event -> 
            let sessionIds = getSessionIdsByEventId <| Guid.Parse event.Id //TODO when old events are removed, change id to guid and remove this parse
            Event.toSummary sessionIds event)
    Array.concat [ events ; eventsBySessionDates ]

//TODO: Once events are complete, remove old events by session date and extra conditional handling.
let getEventDetail (id: string) = 
    try
        match Guid.TryParse id with
        | true, eventId -> 
            let record = EventsProxy.getEvent eventId
            let eventSessions = 
                getSessionsByEventId eventId 
                |> Array.map (fun session -> 
                    let speaker = getSpeaker session.SpeakerId
                    Session.toEventSession speaker session)
            Event.toDetail eventSessions record |> Some
        | false, _ -> 
            getEventDetailByDatedSessionsAndId id |> Some
    with
    | _ -> None
