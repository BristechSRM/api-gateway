namespace Models

open System
open System.Net

type ErrorResponse =
    { HttpStatusCode : HttpStatusCode
      Body : string }

type Result<'Success, 'Failure> =
    | Success of 'Success
    | Failure of 'Failure

type LastContactSummary =
    { Date : DateTime
      SenderId : Guid
      ReceiverId : Guid }

type SessionSummary =
    { Id : Guid
      Title : string
      Status : string
      Date : DateTime option
      SpeakerId : Guid
      SpeakerForename : string
      SpeakerSurname : string
      SpeakerImageUri : string
      SpeakerRating : int
      AdminId : Guid
      AdminForename : string
      AdminSurname : string
      AdminImageUri : string
      LastContact : LastContactSummary option }

type SessionDetail =
    { Id : Guid
      Title : string
      Status : string
      Date : DateTime option
      DateAdded : string
      SpeakerId : Guid
      SpeakerForename : string
      SpeakerSurname : string
      SpeakerImageUri : string
      AdminId : Guid
      AdminForename : string
      AdminSurname : string
      AdminImageUri : string
      LastContact : LastContactSummary option
      ThreadId : Guid }

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

type EventSummary =
    { Id : string
      Date : DateTime
      Description : string
      Location : string
      Sessions : Guid[] }

type EventDetail =
    { Id : string
      Date : DateTime
      Description : string
      Location : string
      Sessions : EventSession[] }