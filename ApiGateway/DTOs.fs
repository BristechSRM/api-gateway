namespace Dtos

open System

type SessionSummaryDto =
    { Id : Guid
      Title : string
      Status : string
      Date : Nullable<DateTime>
      DateAdded : string
      SpeakerId : Guid
      SpeakerForename : string
      SpeakerSurname : string
      SpeakerImageUrl : string
      SpeakerRating : int
      AdminId : Guid
      AdminForename : string
      AdminSurname : string
      AdminImageUrl : string
      ThreadId : Guid }

type LastContactDto =
    { ThreadId : Guid
      Date : DateTime
      SenderId : Guid
      ReceiverId : Guid }