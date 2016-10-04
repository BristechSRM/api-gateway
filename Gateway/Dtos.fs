namespace Dtos

open System

type Speaker =
    { Id : Guid
      Forename : string
      Surname : string
      Rating : int
      ImageUri : string
      Bio : string }

type Admin =
    { Id : Guid
      Forename : string
      Surname : string
      ImageUri : string }

type Session =
    { Id : Guid
      Title : string
      Description : string
      Status : string
      DateAdded : string
      SpeakerId : Guid
      AdminId : Guid option 
      EventId : Guid option }

type Note = 
    { Id : Guid
      SessionId : Guid
      DateAdded : DateTime
      DateModified : DateTime 
      Note : string }

type Handle =
    { Id : int
      ProfileId : Guid
      Type : string
      Identifier : string }

type Profile =
    { Id : Guid
      Forename : string
      Surname : string
      ImageUrl : string
      Rating : int
      Bio : string }

type Event =
    { Id: Guid
      Date: DateTime option
      Name: string 
      MeetupEventId : Guid option }

type MeetupEvent = 
    { Id : Guid
      EventId : Guid
      MeetupId : string
      PublishedDate : DateTime option
      MeetupUrl : string }
