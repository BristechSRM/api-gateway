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

type SessionDto =
    { Id : Guid
      Title : string
      Description : string
      Status : string
      Date : DateTime option
      DateAdded : string
      Speaker : SpeakerSummaryDto
      Admin : AdminSummaryDto option }

type LastContact =
    { Id : string
      Date : DateTime
      ProfileIdOne : Guid
      ProfileIdTwo : Guid }

type Handle =
    { Type : string
      Identifier : string }

type Profile =
    { Id : Guid
      Forename : string
      Surname : string
      ImageUrl : string
      Rating : int
      Bio : string }

type CorrespondenceItemDto = 
    { Id : string
      ExternalId : string
      SenderId : string
      ReceiverId : string
      Date : string
      Message : string
      Type : string
      SenderHandle : string
      ReceiverHandle : string }