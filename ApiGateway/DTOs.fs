namespace Dtos

open System

type SessionSummaryDto =
    { Id : Guid
      Title : string
      Status : string
      Date : string
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
      Date : string
      SenderId : Guid
      ReceiverId : Guid }