module EventsRepository

open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Models

let sessionsUri = "http://api.bris.tech/sessionsummaries/"

let getIds (sessions: SessionSummaryDto[]) =
    sessions
    |> Array.map (fun session -> session.Id)

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
                |> Array.map (fun (date, eventSessions) -> {Date = date; Sessions = getIds eventSessions})
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
                |> Array.filter (fun session -> not <| isNull session.Date && session.Date.Equals(date))
                |> Array.map (fun session -> session.Id)
            let event = { Date = date; Sessions = sessions }
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