namespace Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open HandlesRepository
open ProfilesRepository
open System
open DataTransform

type SpeakersController() =
    inherit ApiController()

    member x.Get(id : Guid) =
        (fun () -> 
            let profile = getProfile id 
            let handlesDtos = getHandlesByProfileId id 
            Profile.toSpeaker handlesDtos profile) 
        |> Catch.respond x HttpStatusCode.OK 