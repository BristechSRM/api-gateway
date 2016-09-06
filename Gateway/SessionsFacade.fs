module SessionsFacade

open System
open AdminFacade
open SessionsProxy
open SpeakerFacade
open LastContactProxy
open EventsFacade
open DataTransform

let getSessionModels() = 
    let sessions = getSessions()
    let lastContacts = getLastContacts()
    sessions |> Seq.map (fun session -> 
        let speaker = getSpeaker session.SpeakerId
        let admin = session.AdminId |> Option.map getAdmin
        let event = None //Event info not needed on dashboard page at the moment. 
        Session.toModel lastContacts speaker admin event session)

let getSessionModel (id : Guid) = 
    let session = getSession id
    let speaker = getSpeaker session.SpeakerId
    let admin = session.AdminId |> Option.map getAdmin
    let event = session.EventId |> Option.bind getEventSummary
    let lastContacts = getLastContacts()
    Session.toModel lastContacts speaker admin event session