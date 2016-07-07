namespace Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open AdminProxy
open System

type AdminsController() =
    inherit ApiController()

    member x.Get(id : Guid) =
        (fun () -> getAdmin id) |> Catch.respond x HttpStatusCode.OK 