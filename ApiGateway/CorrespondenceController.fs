module CorrespondenceController

open Serilog
open System.Net
open System.Net.Http
open System.Web.Http
open Models
open CorrespondenceRepository

type CorrespondenceController() = 
    inherit ApiController()
    
    member x.Get(senderId : string, receiverId : string) = 
        Log.Information("Received get request for correspondence between sender: {senderId} and reciever : {recieverId}", senderId, receiverId) 
        match getCorrespondence(senderId, receiverId) with
          | Success correspondence -> x.Request.CreateResponse(correspondence)
          | Failure error -> x.Request.CreateResponse(error.HttpStatusCode, error.Body)
  