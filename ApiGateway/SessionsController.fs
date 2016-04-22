namespace Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open Serilog
open SessionsRepository

type SessionsController() =
    inherit ApiController()

    member x.Get() =
        Log.Information("Received GET request for sessions")
        let sessions = getSessions()
        x.Request.CreateResponse(sessions)