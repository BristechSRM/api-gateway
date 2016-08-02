module CorrespondenceController

open Serilog
open System.Net
open System.Net.Http
open System.Web.Http
open Models
open CorrespondenceProxy
open DataTransform

type CorrespondenceController() = 
    inherit ApiController()

    member x.Get(senderId : string, receiverId : string) = 
        (fun () -> 
            let correspondence = getCorrespondence senderId receiverId
            correspondence |> Seq.map Correspondence.toModel)
        |> Catch.respond x HttpStatusCode.OK
  