namespace Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open Serilog
open HandlesRepository
open ProfilesRepository
open System
open Models
open DataTransform

type AdminsController() =
    inherit ApiController()

    member x.Get(id : Guid) =
        (fun () -> 
            let profile = getProfile id
            let handlesDtos = getHandlesByProfileId id
            Profile.toAdmin handlesDtos profile) 
        |> Catch.respond x HttpStatusCode.OK 