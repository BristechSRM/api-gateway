module SessionsFacade

open System
open AdminFacade
open SessionsProxy
open SpeakerFacade
open EventsFacade
open DataTransform

let getSessionModels() = 
    let sessions = getSessions()
    sessions |> Seq.map (fun session -> 
        let speaker = getSpeaker session.SpeakerId
        let admin = session.AdminId |> Option.map getAdmin
        let event = None //Event info not needed on dashboard page at the moment. 
        Session.toModel speaker admin event session)

let getSessionModel (id : Guid) = 
    let session = getSession id
    let speaker = getSpeaker session.SpeakerId
    let admin = session.AdminId |> Option.map getAdmin
    let event = session.EventId |> Option.map getEventSummary
    Session.toModel speaker admin event session

let addSessionModel (session : Models.Session) = session |> Session.toDto |> addSession
