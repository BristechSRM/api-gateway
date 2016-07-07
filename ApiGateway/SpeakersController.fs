namespace Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open SpeakerProxy
open System

type SpeakersController() =
    inherit ApiController()

    member x.Get(id : Guid) =
        (fun () -> getSpeaker id) |> Catch.respond x HttpStatusCode.OK 