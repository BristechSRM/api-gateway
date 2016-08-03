module CorrespondenceController

open System.Net
open System.Web.Http
open CorrespondenceProxy
open DataTransform

type CorrespondenceController() = 
    inherit ApiController()

    member x.Get(senderId : string, receiverId : string) = 
        (fun () -> 
            let correspondence = getCorrespondence senderId receiverId
            correspondence |> Seq.map Correspondence.toModel)
        |> Catch.respond x HttpStatusCode.OK
  