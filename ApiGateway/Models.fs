namespace Models

open System

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