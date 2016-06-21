namespace Models

open System
open System.Net

//TODO rename fields for consistency with other services. 
type ServerError =
    { HttpStatusCode : HttpStatusCode
      Body : string }

type Result<'Success, 'Failure> =
    | Success of 'Success
    | Failure of 'Failure

type RawPatchOperation = 
    { Op : string
      Path : string
      Value : string
      From : string }

[<CLIMutable>]
type SpeakerSummary =
    { Id : Guid
      Forename : string
      Surname : string
      Rating : int
      ImageUri : string }

[<CLIMutable>]
type AdminSummary =
    { Id : Guid
      Forename : string
      Surname : string
      ImageUri : string }

[<CLIMutable>]
type LastContactSummary =
    { Date : DateTime
      SenderId : Guid
      ReceiverId : Guid }

[<CLIMutable>]
type SessionSummary =
    { Id : Guid
      Title : string
      Status : string
      Date : DateTime option
      Speaker : SpeakerSummary
      Admin : AdminSummary option
      LastContact : LastContactSummary option }

[<CLIMutable>]
type SessionDetail =
    { Id : Guid
      Title : string
      Status : string
      Date : DateTime option
      DateAdded : string
      Speaker : SpeakerSummary
      Admin : AdminSummary option
      LastContact : LastContactSummary option }

[<CLIMutable>]
type EventSession =
    { Id : Guid
      Title : string
      Description : string
      SpeakerId : Guid
      SpeakerForename : string
      SpeakerSurname : string
      SpeakerBio : string
      SpeakerImageUri : string
      SpeakerRating : int
      StartDate : DateTime option
      EndDate : DateTime option }

[<CLIMutable>]
type EventSummary =
    { Id : string
      Date : DateTime
      Description : string
      Location : string
      Sessions : Guid[] }

[<CLIMutable>]
type EventDetail =
    { Id : string
      Date : DateTime
      Description : string
      Location : string
      Sessions : EventSession[] }

[<CLIMutable>]
type Handle =
    { Type : string
      Identifier : string }

[<CLIMutable>]
type Speaker =
    { Id : Guid
      Forename : string
      Surname : string
      Rating : int
      ImageUri : string
      Bio : string
      Handles : Handle seq }

[<CLIMutable>]
type Admin =
    { Id : Guid
      Forename : string
      Surname : string
      ImageUri : string
      Handles : Handle seq }

[<CLIMutable>]
type CorrespondenceItem = 
    { Id : string
      SenderId : string
      ReceiverId : string
      Date : string
      Message : string
      Type : string
      SenderHandle : string
      ReceiverHandle : string }