module NotesController

open System
open System.Net
open System.Web.Http
open NotesProxy
open DataTransform

type NotesController() = 
    inherit ApiController()

    [<HttpGet>]
    member x.GetBySessionId(sessionId : Guid) = (fun () -> getNotesBySessionId sessionId |> Seq.map Note.toModel) |> Catch.respond x HttpStatusCode.OK
