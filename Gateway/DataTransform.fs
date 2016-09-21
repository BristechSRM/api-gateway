module DataTransform

open System

module Handle = 
    let toHandle (dto : Dtos.Handle) : Models.Handle =
        { Id = dto.Id
          Type = dto.Type
          Identifier = dto.Identifier }

    let toHandleDto (handle : Models.Handle) : Dtos.Handle = 
        { Id = handle.Id
          Type = handle.Type
          Identifier = handle.Identifier }

module Profile = 

    let toAdmin (handles : Dtos.Handle seq) (profile : Dtos.Profile): Models.Admin =
        { Id = profile.Id
          Forename = profile.Forename
          Surname = profile.Surname
          ImageUri = profile.ImageUrl
          Handles = handles |> Seq.map Handle.toHandle }

    let toSpeaker (handles : Dtos.Handle seq) (profile : Dtos.Profile) : Models.Speaker =
        { Id = profile.Id
          Forename = profile.Forename
          Surname = profile.Surname
          Rating = profile.Rating
          ImageUri = profile.ImageUrl
          Bio = profile.Bio
          Handles = handles |> Seq.map Handle.toHandle }

module Session = 

    let private convertToLastContactSummary (dto : Dtos.LastContact) : Models.LastContact =
        { Date = dto.Date; SenderId = dto.ProfileIdOne; ReceiverId = dto.ProfileIdTwo }

    let private getLastContact (senderId : Guid) (receiverId : Guid) (lastContacts : Dtos.LastContact[]) =
        lastContacts
        |> Seq.tryFind (fun lastContact -> (lastContact.ProfileIdOne.Equals senderId && lastContact.ProfileIdTwo.Equals receiverId) || (lastContact.ProfileIdOne.Equals receiverId && lastContact.ProfileIdTwo.Equals senderId))
        |> Option.map convertToLastContactSummary

    let toModel lastContacts speaker admin event (session : Dtos.Session) : Models.Session =
        { Id = session.Id
          Title = session.Title
          Description = session.Description
          Status = session.Status
          DateAdded = session.DateAdded
          Speaker = speaker
          Admin = admin
          Event = event
          LastContact = 
            match session.AdminId with 
            | Some aid -> getLastContact aid session.SpeakerId lastContacts
            | None -> None }

    let toDto (session : Models.Session) : Dtos.Session = 
        { Id = session.Id
          Title = session.Title
          Description = session.Description
          Status = session.Status
          DateAdded = session.DateAdded
          SpeakerId = session.Speaker.Id
          AdminId = session.Admin |> Option.map (fun admin -> admin.Id)
          EventId = session.Event |> Option.map (fun event -> event.Id) } 

    let toEventSession (speaker : Models.Speaker) (session : Dtos.Session) : Models.EventSession = 
        { Id = session.Id
          Title = session.Title
          Description = session.Description
          SpeakerId = speaker.Id
          SpeakerForename = speaker.Forename
          SpeakerSurname = speaker.Surname
          SpeakerBio = speaker.Bio
          SpeakerImageUri = speaker.ImageUri
          SpeakerRating = speaker.Rating }

module Correspondence = 
    let toModel (correspondenceItem : Dtos.CorrespondenceItem) : Models.CorrespondenceItem =
        { Id = correspondenceItem.Id
          SenderId = correspondenceItem.SenderId
          ReceiverId = correspondenceItem.ReceiverId
          Date = correspondenceItem.Date
          Message = correspondenceItem.Message
          Type = correspondenceItem.Type
          SenderHandle = correspondenceItem.SenderHandle
          ReceiverHandle = correspondenceItem.ReceiverHandle }

module Event =
    let toSummary sessionIds meetupEvent (event: Dtos.Event)  : Models.EventSummary =
        { Id = event.Id
          Date = event.Date
          Description = event.Name
          Location = ""
          MeetupEvent = meetupEvent
          SessionIds = sessionIds }

    let toDetail eventSessions (event: Dtos.Event) : Models.EventDetail =
        { Id = event.Id
          Date = event.Date 
          Description = event.Name
          Location = ""
          Sessions = eventSessions }

    let toDto (event: Models.Event) : Dtos.Event =
        { Id = event.Id 
          Date = event.Date
          Name = event.Name 
          MeetupEventId = event.MeetupEventId }

    let toModel (event: Dtos.Event) : Models.Event =
        { Id = event.Id 
          Date = event.Date
          Name = event.Name 
          MeetupEventId = event.MeetupEventId }

module MeetupEvent =
    let toDto (me: Models.MeetupEvent) : Dtos.MeetupEvent =
        { Id = me.Id 
          EventId = me.EventId
          MeetupId = me.MeetupId
          PublishedDate = me.PublishedDate
          MeetupUrl = me.MeetupUrl }

    let toModel (me : Dtos.MeetupEvent) : Models.MeetupEvent =
        { Id = me.Id 
          EventId = me.EventId
          MeetupId = me.MeetupId 
          PublishedDate = me.PublishedDate
          MeetupUrl = me.MeetupUrl }
