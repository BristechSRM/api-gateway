module SessionsRepository

open System
open System.Net.Http
open Newtonsoft.Json
open DTOs
open Models

let convertToLastContactSummary (dto : LastContactDTO) : LastContactSummary =
    new LastContactSummary(dto.Date, dto.SenderId, dto.ReceiverId)

let convertToSessionSummary (lastContacts, session: SessionSummaryDTO) : SessionSummary =
    let lastContact =
        match lastContacts |> Seq.tryFind (fun lastContact -> lastContact.ThreadId.Equals session.ThreadId) with
        | Some lastContact -> convertToLastContactSummary lastContact
        | None -> null
    { Id = session.Id
      Title = session.Title
      Status = session.Status
      Date = session.Date
      SpeakerId = session.SpeakerId
      SpeakerForename = session.SpeakerForename
      SpeakerSurname = session.SpeakerSurname
      SpeakerImageUri = session.SpeakerImageUrl
      SpeakerRating = session.SpeakerRating
      AdminId = session.AdminId
      AdminForename = session.AdminForename
      AdminSurname = session.AdminSurname
      AdminImageUri = session.AdminImageUrl
      LastContact = lastContact }

let getLastContacts() =
    use client = new HttpClient()

    try
        let lastContactJson = client.GetAsync("http://api.bris.tech:8080/last-contact").Result.Content.ReadAsStringAsync().Result
        JsonConvert.DeserializeObject<LastContactDTO[]>(lastContactJson)
    with
        // Endpoint not found
        | :? AggregateException -> [||]

let getSessions() = 
    use client = new HttpClient()

    let sessions =
        try
            let sessionJson = client.GetAsync("http://api.bris.tech/sessionsummaries").Result.Content.ReadAsStringAsync().Result
            Some (JsonConvert.DeserializeObject<SessionSummaryDTO[]>(sessionJson))
        with
            // Endpoint not found
            | :? AggregateException -> None
    
    match sessions with
        | Some sessions ->
            let lastContacts = getLastContacts()
            Some (sessions |> Seq.map (fun session -> convertToSessionSummary(lastContacts, session)))
        | None -> None

