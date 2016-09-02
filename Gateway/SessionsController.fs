namespace Controllers

open DataTransform
open RestModels
open SessionsProxy
open System
open System.Net
open System.Web.Http
open SessionsFacade

type SessionsController() = 
    inherit ApiController()
    
    member x.Get() = (fun () -> getSessionModels()) |> Catch.respond x HttpStatusCode.OK
    
    member x.Get(id : Guid) = (fun () -> getSessionModel id) |> Catch.respond x HttpStatusCode.OK

    member x.Post(session : Models.Session) = (fun () -> session |> Session.toDto |> addSession) |> Catch.respond x HttpStatusCode.Created
    
    member x.Patch(id : Guid, op : PatchOp) = (fun () -> patchSession id op) |> Catch.respond x HttpStatusCode.NoContent
