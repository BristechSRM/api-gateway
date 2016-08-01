namespace Controllers

open AdminProxy
open DataTransform
open LastContactProxy
open RestModels
open Serilog
open SessionsProxy
open SpeakerProxy
open System
open System.Net
open System.Net.Http
open System.Web.Http

type SessionsController() = 
    inherit ApiController()
    
    member x.Get() = 
        (fun () -> 
            let sessions = getSessions()
            let lastContacts = getLastContacts()
            sessions |> Seq.map (fun session -> 
                let speaker = getSpeaker session.SpeakerId
                let admin = session.AdminId |> Option.map getAdmin
                Session.toModel lastContacts speaker admin session))
        |> Catch.respond x HttpStatusCode.OK
    
    member x.Get(id : Guid) = 
        (fun () -> 
            let session = getSession id
            let speaker = getSpeaker session.SpeakerId
            let admin = session.AdminId |> Option.map getAdmin
            let lastContacts = getLastContacts()
            Session.toModel lastContacts speaker admin session)
        |> Catch.respond x HttpStatusCode.OK

    member x.Post(session : Models.Session) = (fun () -> session |> Session.toDto |> addSession) |> Catch.respond x HttpStatusCode.Created
    
    member x.Patch(id : Guid, op : PatchOp) = (fun () -> patchSession id op) |> Catch.respond x HttpStatusCode.NoContent
