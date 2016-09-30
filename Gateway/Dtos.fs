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

type LastContact =
    { Id : string
      Date : DateTime
      ProfileIdOne : Guid
      ProfileIdTwo : Guid }

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

type CorrespondenceItem =
    { Id : string
      ExternalId : string
      SenderId : string
      ReceiverId : string
      Date : string
      Message : string
      Type : string
      SenderHandle : string
      ReceiverHandle : string }

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
