module SessionsRepository
open Bristech.Srm.HttpConfig
open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Models

let sessionsUri = "http://api.bris.tech/sessions/"
let lastContactUri = "http://api.bris.tech:8080/last-contact/"

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
    { Date = dto.Date; SenderId = dto.SenderId; ReceiverId = dto.ReceiverId }

let getLastContact (threadId, lastContacts : LastContactDto[]) =
    match lastContacts |> Seq.tryFind (fun lastContact -> lastContact.ThreadId.Equals threadId) with
        | Some lastContact -> Some(convertToLastContactSummary lastContact)
        | None -> None

let convertToSessionSummary (lastContacts : LastContactDto[], session : SessionSummaryDto) : SessionSummary =
    { Id = session.Id
      Title = session.Title
      Status = session.Status
      Date = session.Date
      Speaker = session.Speaker |> convertToSpeakerSummary
      Admin =
        match session.Admin with
        | Some admin -> admin |>  convertToAdminSummary |> Some
        | None -> None
      LastContact = getLastContact(session.ThreadId, lastContacts) }

let convertToSessionDetail (lastContacts : LastContactDto[], session : SessionSummaryDto) : SessionDetail =
    { Id = session.Id
      Title = session.Title
      Status = session.Status
      Date = session.Date
      DateAdded = session.DateAdded
      Speaker = session.Speaker |> convertToSpeakerSummary
      Admin =
        match session.Admin with
        | Some admin -> admin |>  convertToAdminSummary |> Some
        | None -> None
      LastContact = getLastContact(session.ThreadId, lastContacts)
      ThreadId = session.ThreadId }

let getLastContacts() =
    use client = new HttpClient()

    try
        let lastContactJson = client.GetAsync(lastContactUri).Result.Content.ReadAsStringAsync().Result
        Log.Information("Last contact endpoint found")
        JsonConvert.DeserializeObject<LastContactDto[]>(lastContactJson)
    with
        | :? AggregateException ->
            Log.Information("Could not reach last contact endpoint")
            [||]

let getSessions() = 
    use client = new HttpClient()

    try
        let result = client.GetAsync(sessionsUri).Result
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
        | :? AggregateException ->
            Log.Information("Could not reach sessions endpoint")
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "Could not reach sessions endpoint" }
        | ex ->
            Log.Information("Unhandled exception: {message}", ex.Message)
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred: " + ex.Message }

let getSession(id : Guid) =
    use client = new HttpClient()
    
    try
        let result = client.GetAsync(sessionsUri + id.ToString()).Result
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
        | :? AggregateException ->
            Log.Information("Could not reach session endpoint")
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "Could not reach sessions endpoint" }
        | ex ->
            Log.Information("Unhandled exception: {message}", ex.Message)
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred: " + ex.Message }
