namespace Dtos

open System

type SpeakerSummaryDto =
    { Id : Guid
      Forename : string
      Surname : string
      Rating : int
      ImageUri : string }

type AdminSummaryDto =
    { Id : Guid
      Forename : string
      Surname : string
      ImageUri : string }

type SessionSummaryDto =
    { Id : Guid
      Title : string
      Status : string
      Date : DateTime option
      DateAdded : string
      Speaker : SpeakerSummaryDto
      Admin : AdminSummaryDto option
      ThreadId : Guid }

type LastContactDto =
    { ThreadId : Guid
      Date : DateTime
      SenderId : Guid
      ReceiverId : Guid }