namespace Dtos

open System

type SpeakerSummaryDto =
    { Id : Guid
      Forename : string
      Surname : string
      Rating : int
      ImageUri : string
      Bio : string }

type AdminSummaryDto =
    { Id : Guid
      Forename : string
      Surname : string
      ImageUri : string }

type SessionSummaryDto =
    { Id : Guid
      Title : string
      Description : string
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

type HandleDto =
    { Type : string
      Identifier : string }

type ProfileDto =
    { Id : Guid
      Forename : string
      Surname : string
      ImageUrl : string
      Rating : int
      Bio : string
      Handles : HandleDto[] }