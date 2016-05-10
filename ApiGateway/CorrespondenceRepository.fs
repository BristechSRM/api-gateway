module CorrespondenceRepository

open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Models

let correspondenceUri = "http://comms:8080/correspondence"

let convertToCorrespondenceItem (correspondenceItem : CorrespondenceItemDto) : CorrespondenceItem =
    { Id = correspondenceItem.Id
      SenderId = correspondenceItem.SenderId
      ReceiverId = correspondenceItem.ReceiverId
      Date = correspondenceItem.Date
      Message = correspondenceItem.Message
      Type = correspondenceItem.Type
      SenderHandle = correspondenceItem.SenderHandle
      ReceiverHandle = correspondenceItem.ReceiverHandle }

let getCorrespondence(senderId : string, receiverId : string) =
    use client = new HttpClient()

    try
        let uri = String.Format("{0}?profileIdOne={1}&profileIdTwo={2}", correspondenceUri, senderId, receiverId)
        let result = client.GetAsync(uri).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let correspondenceJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Correspondence endpoint found")
            JsonConvert.DeserializeObject<CorrespondenceItemDto[]>(correspondenceJson)
            |> Seq.map convertToCorrespondenceItem
            |> Success
        | _ ->
            Log.Information("Status code: {statusCode}. Reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase)
            Failure { HttpStatusCode = result.StatusCode; Body = result.ReasonPhrase }
    with
        | :? AggregateException ->
            Log.Information("Could not reach correspondence endpoint")
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "Could not reach correspondence endpoint" }
        | ex ->
            Log.Information("Unhandled exception: {message}", ex.Message)
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred: " + ex.Message }