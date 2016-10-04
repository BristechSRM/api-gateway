namespace Models

open System

[<CLIMutable>]
type Handle =
    { Id : int 
      Type : string
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
type MeetupEvent = 
    { Id : Guid
      EventId : Guid
      MeetupId : string
      PublishedDate : DateTime option
      MeetupUrl : string }

[<CLIMutable>]
type Event =
    { Id: Guid
      Date: DateTime option
      Name: string 
      MeetupEventId : Guid option } 

[<CLIMutable>]
type EventSummary =
    { Id : Guid
      Date : DateTime option
      Description : string
      Location : string
      MeetupEvent : MeetupEvent option
      SessionIds : Guid[] }

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
      SpeakerRating : int }

[<CLIMutable>]
type EventDetail =
    { Id : Guid
      Date : DateTime option
      Description : string
      Location : string
      Sessions : EventSession[] }

[<CLIMutable>]
type Session =
    { Id : Guid
      Title : string
      Description : string
      Status : string
      DateAdded : string
      Speaker : Speaker
      Admin : Admin option
      Event : EventSummary option }

[<CLIMutable>]
type Note = 
    { Id : Guid
      SessionId : Guid
      DateAdded : DateTime
      DateModified : DateTime 
      Note : string }
