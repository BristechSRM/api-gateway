module EventsProxy

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

let convertToISO8601 (datetime : DateTime) =
    datetime.ToString("yyyy-MM-ddTHH\:mm\:ss\Z")

let getIds (sessions: Dtos.Session[]) =
    sessions
    |> Array.map (fun session -> session.Id)

let onSameDay (datetime1: DateTime) (datetime2: DateTime) =
    datetime1.Date = datetime2.Date

//TODO these can be refactored to use Sessions Proxy
let getEvents() = 
    use client = new HttpClient()

    try
        let result = client.GetAsync(sessionsUrl).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let sessionJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Sessions endpoint found")
            let sessions = JsonConvert.DeserializeObject<Dtos.Session[]>(sessionJson)
            let events =
                sessions
                |> Array.filter (fun session -> session.Date.IsSome)
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
        let result = client.GetAsync(sessionsUrl).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let sessionJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Sessions endpoint found")
            let sessions =
                JsonConvert.DeserializeObject<Dtos.Session[]>(sessionJson)
                |> Array.filter (fun session -> session.Date.IsSome && onSameDay session.Date.Value date)
                |> Array.map (fun session -> 
                    let speaker = getSpeaker session.SpeakerId
                    Session.toEventSession speaker session)
            let event = { EventDetail.Id = date.Date |> convertToISO8601; Date = date; Description = ""; Location = ""; Sessions = sessions }
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
