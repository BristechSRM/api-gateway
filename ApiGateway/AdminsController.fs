namespace Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open Serilog
open ProfilesRepository
open System
open Models
open DataTransform

type AdminsController() =
    inherit ApiController()

    //TODO handles
    member x.Get(id : Guid) =
        Catch.respond x HttpStatusCode.OK (fun () -> getProfile id |> Profile.toAdmin [])