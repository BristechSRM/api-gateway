﻿module EventsRepository

open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Models

let sessionsUri = "http://api.bris.tech/sessions/"

let convertToISO8601 (datetime : DateTime) =
    datetime.ToString("yyyy-MM-ddTHH\:mm\:ss\Z")

let getIds (sessions: SessionSummaryDto[]) =
    sessions
    |> Array.map (fun session -> session.Id)

let onSameDay (datetime1: DateTime) (datetime2: DateTime) =
    datetime1.Date = datetime2.Date

let convertToEventSession (session: SessionSummaryDto) =
    let startDate = Option.ofNullable session.Date
    let endDate =
        match startDate with
        | Some date -> Some <| date.AddHours(1.0)
        | None -> None
    { Id = session.Id
      Title = session.Title
      Description = ""
      SpeakerId = session.SpeakerId
      SpeakerForename = session.SpeakerForename
      SpeakerSurname = session.SpeakerSurname
      SpeakerBio = ""
      SpeakerImageUri = session.SpeakerImageUrl
      SpeakerRating = session.SpeakerRating
      StartDate = startDate
      EndDate = endDate }

let getEvents() = 
    use client = new HttpClient()

    try
        let result = client.GetAsync(sessionsUri).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let sessionJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Sessions endpoint found")
            let sessions = JsonConvert.DeserializeObject<SessionSummaryDto[]>(sessionJson)
            let events =
                sessions
                |> Array.filter (fun session -> session.Date.HasValue)
                |> Array.groupBy (fun session -> session.Date.Value)
                |> Array.map (fun (date, eventSessions) -> {EventSummary.Id = date.Date |> convertToISO8601; Date = date; Description = ""; Location = ""; Sessions = getIds eventSessions})
            Success(events)
        | _ ->
            Log.Information("Status code: {statusCode}. Reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase)
            Failure { HttpStatusCode = result.StatusCode; Body = result.ReasonPhrase }
    with
        // Endpoint not found
        | :? AggregateException ->
            Log.Information("Could not reach sessions endpoint")
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "Could not reach sessions endpoint" }
        | ex ->
            Log.Information("Unhandled exception: {message}", ex.Message)
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred: " + ex.Message }

let getEvent(id) =
    use client = new HttpClient()

    try
        let date = DateTime.Parse(id, null, System.Globalization.DateTimeStyles.RoundtripKind)
        let result = client.GetAsync(sessionsUri).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let sessionJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Sessions endpoint found")
            let sessions =
                JsonConvert.DeserializeObject<SessionSummaryDto[]>(sessionJson)
                |> Array.filter (fun session -> session.Date.HasValue && onSameDay session.Date.Value date)
                |> Array.map convertToEventSession
            let event = { Id = date.Date |> convertToISO8601; Date = date; Description = ""; Location = ""; Sessions = sessions }
            Success(event)
        | _ ->
            Log.Information("Status code: {statusCode}. Reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase)
            Failure { HttpStatusCode = result.StatusCode; Body = result.ReasonPhrase }
    with
        // Endpoint not found
        | :? AggregateException ->
            Log.Information("Could not reach sessions endpoint")
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "Could not reach sessions endpoint" }
        | :? FormatException ->
            Log.Information("Event not found")
            Failure { HttpStatusCode = HttpStatusCode.NotFound; Body = "Event not found" }
        | ex ->
            Log.Information("Unhandled exception: {message}", ex.Message)
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred: " + ex.Message }