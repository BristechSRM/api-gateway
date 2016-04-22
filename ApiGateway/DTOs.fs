namespace DTOs

open System

type SessionDTO =
    { Id : Guid
      Title : string
      Status : string
      Date : string
      SpeakerId : Guid
      AdminId : Guid
      ThreadId : Guid
      DateAdded : string }

type SessionSummaryDTO =
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

type LastContactDTO =
    { ThreadId : Guid
      Date : string
      SenderId : Guid
      ReceiverId : Guid }