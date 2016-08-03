namespace Controllers

open System.Web.Http
open System.Net
open EventsFacade

type EventsController() =
    inherit ApiController()

    member x.Get() = (fun () -> getEvents()) |> Catch.respond x HttpStatusCode.OK

    member x.Get(id : string) = (fun () -> getEvent id) |> Catch.respond x HttpStatusCode.OK
