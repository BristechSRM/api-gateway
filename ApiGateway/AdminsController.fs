namespace Controllers

open System.Net.Http
open System.Web.Http
open Serilog
open ProfilesRepository
open System
open Models

type AdminsController() =
    inherit ApiController()

    member x.Get(id : Guid) =
        Log.Information("Received GET request for an admin with id {id}", id)
        match getAdmin id with
        | Success admin -> x.Request.CreateResponse(admin)
        | Failure error -> x.Request.CreateResponse(error.HttpStatusCode, error.Body)