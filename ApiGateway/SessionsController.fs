namespace Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open Serilog
open LastContactRepository
open SpeakerRepository
open AdminRepository
open SessionsRepository
open DataTransform
open System

type SessionsController() =
    inherit ApiController()

    member x.Get() =
        (fun () -> 
            let sessions = getSessions()
            let lastContacts = getLastContacts()
            sessions |> Seq.map (fun session -> 
                let speaker = getSpeakerSummary session.SpeakerId
                let admin = session.AdminId |> Option.map getAdminSummary
                Session.toModel lastContacts speaker admin session)) 
        |> Catch.respond x HttpStatusCode.OK 


    member x.Get(id : Guid) =
        (fun () -> 
            let session = getSession id
            let speaker = getSpeakerSummary session.SpeakerId
            let admin = session.AdminId |> Option.map getAdminSummary
            let lastContacts = getLastContacts()
            Session.toModel lastContacts speaker admin session) 
        |> Catch.respond x HttpStatusCode.OK
