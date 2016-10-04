module NotesController

open System
open System.Net
open System.Web.Http
open NotesProxy
open DataTransform
open RestModels

type NotesController() = 
    inherit ApiController()

    [<HttpGet>]
    member x.GetBySessionId(sessionId : Guid) = (fun () -> getNotesBySessionId sessionId |> Seq.map Note.toModel) |> Catch.respond x HttpStatusCode.OK

    member x.Post(note : Models.Note) = (fun () -> note |> Note.toDto |> postNote) |> Catch.respond x HttpStatusCode.Created

    member x.Patch(id : Guid, op : PatchOp) = (fun () -> patchNote id op) |> Catch.respond x HttpStatusCode.NoContent
