namespace Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open Serilog
open LastContactRepository
open SessionsRepository
open DataTransform
open System

type SessionsController() =
    inherit ApiController()

    member x.Get() =
        (fun () -> 
            let sessions = getSessions()
            let lastContacts = getLastContacts()
            sessions |> Seq.map (fun session -> Session.toModel(lastContacts, session))) 
        |> Catch.respond x HttpStatusCode.OK 


    member x.Get(id : Guid) =
        (fun () -> 
            let session = getSession id
            let lastContacts = getLastContacts()
            Session.toModel(lastContacts, session)) 
        |> Catch.respond x HttpStatusCode.OK
