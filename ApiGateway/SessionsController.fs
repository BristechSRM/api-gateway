namespace Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open Serilog
open SessionsRepository
open System
open Models

type SessionsController() =
    inherit ApiController()

    member x.Get() =
        (fun () -> getSessions()) |> Catch.respond x HttpStatusCode.OK 
//        Log.Information("Received GET request for sessions")
//        match getSessions() with
//        | Success sessions -> x.Request.CreateResponse(sessions)
//        | Failure error -> x.Request.CreateResponse(error.HttpStatusCode, error.Body)

    member x.Get(id : Guid) =
        (fun () -> getSession id) |> Catch.respond x HttpStatusCode.OK
//        Log.Information("Received GET request for a session with id {id}", id)
//        match getSession id with
//        | Success session -> x.Request.CreateResponse(session)
//        | Failure error -> x.Request.CreateResponse(error.HttpStatusCode, error.Body)