namespace Models

open System
open System.Net

type ErrorResponse =
    { HttpStatusCode : HttpStatusCode
      Body : string }

type Result<'Success> =
    | Success of 'Success
    | Failure of ErrorResponse

[<AllowNullLiteral>]
type LastContactSummary(date, senderId, receiverId) =
    member x.Date with get() = date
    member x.SenderId with get() = senderId
    member x.ReceiverId with get() = receiverId

type SessionSummary =
    { Id : Guid
      Title : string
      Status : string
      Date : string
      SpeakerId : Guid
      SpeakerForename : string
      SpeakerSurname : string
      SpeakerImageUri : string
      SpeakerRating : int
      AdminId : Guid
      AdminForename : string
      AdminSurname : string
      AdminImageUri : string
      LastContact : LastContactSummary }

type SessionDetail =
    { Id : Guid
      Title : string
      Status : string
      Date : string
      DateAdded : string
      SpeakerId : Guid
      SpeakerForename : string
      SpeakerSurname : string
      SpeakerImageUri : string
      AdminId : Guid
      AdminForename : string
      AdminSurname : string
      AdminImageUri : string
      LastContact : LastContactSummary
      ThreadId : Guid }

type EventSummary =
    { Date : string
      Sessions : Guid seq }