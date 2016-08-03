module EventsFacade

open Config
open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Models
open DataTransform
open SpeakerFacade
open SessionsProxy

let convertToISO8601 (datetime : DateTime) =
    datetime.ToString("yyyy-MM-ddTHH\:mm\:ss\Z")

let getIds (sessions: Dtos.Session[]) =
    sessions
    |> Array.map (fun session -> session.Id)

let onSameDay (datetime1: DateTime) (datetime2: DateTime) =
    datetime1.Date = datetime2.Date

let getEvents() = 
    getSessions()
    |> Array.filter (fun session -> session.Date.IsSome)
    |> Array.groupBy (fun session -> session.Date.Value)
    |> Array.map (fun (date, eventSessions) -> {EventSummary.Id = date.Date |> convertToISO8601; Date = date; Description = ""; Location = ""; Sessions = getIds eventSessions})      

let getEvent(id) = 
    let date = DateTime.Parse(id, null, System.Globalization.DateTimeStyles.RoundtripKind)
    let eventSessions = 
        getSessions()
        |> Array.filter (fun session -> session.Date.IsSome && onSameDay session.Date.Value date)
        |> Array.map (fun session -> 
            let speaker = getSpeaker session.SpeakerId
            Session.toEventSession speaker session)
    let event = { EventDetail.Id = date.Date |> convertToISO8601; Date = date; Description = ""; Location = ""; Sessions = eventSessions }
    event
