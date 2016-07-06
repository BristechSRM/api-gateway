module SessionsRepository

open Config
open Bristech.Srm.HttpConfig
open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Models
open LastContactRepository

let convertToSpeakerSummary (dto : Dtos.SpeakerSummary) : Models.SpeakerSummary =
    { Id = dto.Id
      Forename = dto.Forename
      Surname = dto.Surname
      Rating = dto.Rating
      ImageUri = dto.ImageUri }

let convertToAdminSummary (dto : Dtos.AdminSummary) : Models.AdminSummary =
    { Id = dto.Id
      Forename = dto.Forename
      Surname = dto.Surname
      ImageUri = dto.ImageUri }

let convertToLastContactSummary (dto : Dtos.LastContact) : Models.LastContactSummary =
    { Date = dto.Date; SenderId = dto.ProfileIdOne; ReceiverId = dto.ProfileIdTwo }


let getLastContact (senderId : Guid, receiverId : Guid, lastContacts : Dtos.LastContact[]) =
    try
        lastContacts
        |> Seq.tryFind (fun lastContact -> (lastContact.ProfileIdOne.Equals senderId && lastContact.ProfileIdTwo.Equals receiverId) || (lastContact.ProfileIdOne.Equals receiverId && lastContact.ProfileIdTwo.Equals senderId))
        |> Option.map convertToLastContactSummary
    with
    | ex ->
        Log.Error("getLastContact(id,id,contacts) - Exception: {ex}", ex)
        None

let convertToSessionDetail (lastContacts : Dtos.LastContact[], session : Dtos.Session) : Models.Session =
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

let getSessions() =  
    use client = new HttpClient()

    try
        let result = client.GetAsync(sessionsUrl).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let sessionJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Sessions endpoint found")
            let sessions = JsonConvert.DeserializeObject<Dtos.Session[]>(sessionJson)
            let lastContacts = getLastContacts()
            Success(sessions |> Seq.map (fun session -> convertToSessionDetail(lastContacts, session)))
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
            let session = JsonConvert.DeserializeObject<Dtos.Session>(sessionJson)
            let lastContacts = getLastContacts()
            Success(convertToSessionDetail(lastContacts, session))
        | _ ->
            Log.Information("Status code: {statusCode}. Reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase)
            Failure { HttpStatusCode = result.StatusCode; Body = result.ReasonPhrase }
    with
    | ex ->
        Log.Error("Exception: {ex}", ex)
        Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred: " + ex.Message }
