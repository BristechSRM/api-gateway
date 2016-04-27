﻿module EventsRepository

open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Models

let sessionsUri = "http://api.bris.tech/sessionsummaries/"

let convertToDateTime iso =
    DateTime.Parse(iso, null, System.Globalization.DateTimeStyles.RoundtripKind)

let convertToISO8601 (datetime : DateTime) =
    datetime.ToString("yyyy-MM-ddTHH\:mm\:ss\Z")

let getIds (sessions: SessionSummaryDto[]) =
    sessions
    |> Array.map (fun session -> session.Id)

let onSameDay iso1 iso2 =
    let datetime1 = convertToDateTime iso1
    let datetime2 = convertToDateTime iso2
    datetime1.Date = datetime2.Date

let convertToEventSession (session: SessionSummaryDto) =
    let endDate =
        (convertToDateTime session.Date).AddHours(1.0)
        |> convertToISO8601
    { Id = session.Id
      Title = session.Title
      Description = ""
      SpeakerId = session.SpeakerId
      SpeakerForename = session.SpeakerForename
      SpeakerSurname = session.SpeakerSurname
      SpeakerBio = ""
      SpeakerImageUri = session.SpeakerImageUrl
      SpeakerRating = session.SpeakerRating
      StartDate = session.Date
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
                |> Array.filter (fun session -> not <| isNull session.Date)
                |> Array.groupBy (fun session -> session.Date)
                |> Array.map (fun (date, eventSessions) -> {EventSummary.Id = date; Date = date; Description = ""; Location = ""; Sessions = getIds eventSessions})
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

let getEvent(date) =
    use client = new HttpClient()

    try
        let result = client.GetAsync(sessionsUri).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let sessionJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Sessions endpoint found")
            let sessions =
                JsonConvert.DeserializeObject<SessionSummaryDto[]>(sessionJson)
                |> Array.filter (fun session -> not <| isNull session.Date && onSameDay session.Date date)
                |> Array.map convertToEventSession
            let event = { Id = date; Date = date; Description = ""; Location = ""; Sessions = sessions }
            Success(event)
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