﻿module SessionsRepository

open Config
open Bristech.Srm.HttpConfig
open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Models

let convertToSpeakerSummary (dto : SpeakerSummaryDto) : SpeakerSummary =
    { Id = dto.Id
      Forename = dto.Forename
      Surname = dto.Surname
      Rating = dto.Rating
      ImageUri = dto.ImageUri }

let convertToAdminSummary (dto : AdminSummaryDto) : AdminSummary =
    { Id = dto.Id
      Forename = dto.Forename
      Surname = dto.Surname
      ImageUri = dto.ImageUri }

let convertToLastContactSummary (dto : LastContactDto) : LastContactSummary =
    { Date = dto.Date; SenderId = dto.ProfileIdOne; ReceiverId = dto.ProfileIdTwo }


let getLastContact (senderId : Guid, receiverId : Guid, lastContacts : LastContactDto[]) =
    try
        lastContacts
        |> Seq.tryFind (fun lastContact -> (lastContact.ProfileIdOne.Equals senderId && lastContact.ProfileIdTwo.Equals receiverId) || (lastContact.ProfileIdOne.Equals receiverId && lastContact.ProfileIdTwo.Equals senderId))
        |> Option.map convertToLastContactSummary
    with
    | ex ->
        Log.Error("getLastContact(id,id,contacts) - Exception: {ex}", ex)
        None

let convertToSessionSummary (lastContacts : LastContactDto[], session : SessionSummaryDto) : SessionSummary =
    let spk = session.Speaker |> convertToSpeakerSummary
    let adm = session.Admin |> Option.map convertToAdminSummary
    let lc =
        match adm with 
        | Some admin -> getLastContact(admin.Id,spk.Id,lastContacts)
        | None -> None
    { Id = session.Id
      Title = session.Title
      Status = session.Status
      Date = session.Date
      Speaker = spk
      Admin = adm
      LastContact = lc }

let convertToSessionDetail (lastContacts : LastContactDto[], session : SessionSummaryDto) : SessionDetail =
    let spk = session.Speaker |> convertToSpeakerSummary
    let adm = session.Admin |> Option.map convertToAdminSummary
    let lc =
        match adm with 
        | Some admin -> getLastContact(admin.Id,spk.Id,lastContacts)
        | None -> None
    { Id = session.Id
      Title = session.Title
      Status = session.Status
      Date = session.Date
      DateAdded = session.DateAdded
      Speaker = spk
      Admin = adm
      LastContact = lc }

let getLastContacts() =
    use client = new HttpClient()

    try
        let lastContactJson = client.GetAsync(lastContactUrl).Result.Content.ReadAsStringAsync().Result
        Log.Information("Last contact endpoint found")
        JsonConvert.DeserializeObject<LastContactDto[]>(lastContactJson)
    with
    | ex ->
        Log.Error("getLastContacts() - Exception: {ex}", ex)
        [||]

let getSessions() = 
    use client = new HttpClient()

    try
        let result = client.GetAsync(sessionsUrl).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let sessionJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Sessions endpoint found")
            let sessions = JsonConvert.DeserializeObject<SessionSummaryDto[]>(sessionJson)
            let lastContacts = getLastContacts()
            Success(sessions |> Seq.map (fun session -> convertToSessionSummary(lastContacts, session)))
        | _ ->
            Log.Information("Status code: {statusCode}. Reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase)
            Failure { HttpStatusCode = result.StatusCode; Body = result.ReasonPhrase }
    with
    | ex ->
        Log.Error("getSessions() - Exception: {ex}", ex)
        Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred: " + ex.Message }

let getSession(id : Guid) =
    use client = new HttpClient()
    
    try
        let result = client.GetAsync(sessionsUrl + id.ToString()).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let sessionJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Session endpoint found")
            let session = JsonConvert.DeserializeObject<SessionSummaryDto>(sessionJson)
            let lastContacts = getLastContacts()
            Success(convertToSessionDetail(lastContacts, session))
        | _ ->
            Log.Information("Status code: {statusCode}. Reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase)
            Failure { HttpStatusCode = result.StatusCode; Body = result.ReasonPhrase }
    with
    | ex ->
        Log.Error("Exception: {ex}", ex)
        Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred: " + ex.Message }
