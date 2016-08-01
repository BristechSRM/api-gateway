namespace Controllers

open ProfilesProxy
open RestModels
open SpeakerFacade
open System
open System.Net
open System.Net.Http
open System.Web.Http

type SpeakersController() = 
    inherit ApiController()

    member x.Get() = (fun () -> getSpeakers ()) |> Catch.respond x HttpStatusCode.OK

    member x.Get(id : Guid) = (fun () -> getSpeaker id) |> Catch.respond x HttpStatusCode.OK

    member x.Patch(id : Guid, op : PatchOp) = (fun () -> patchProfile id op) |> Catch.respond x HttpStatusCode.NoContent
