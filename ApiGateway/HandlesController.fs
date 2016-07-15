namespace Controllers

open HandlesProxy
open RestModels
open System.Net
open System.Net.Http
open System.Web.Http

type HandlesController() = 
    inherit ApiController()

    member x.Get(id : int) = (fun () -> getHandle id) |> Catch.respond x HttpStatusCode.OK

    member x.Patch(id : int, op : PatchOp) = (fun () -> patchHandle id op) |> Catch.respond x HttpStatusCode.NoContent
