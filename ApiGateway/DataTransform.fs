module DataTransform
open System
open Serilog

module Handle = 
    let toHandle (dto : Dtos.Handle) : Models.Handle =
        { Type = dto.Type
          Identifier = dto.Identifier }

    let toHandleDto (handle : Models.Handle) : Dtos.Handle = 
        { Type = handle.Type
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

    let private convertToSpeakerSummary (dto : Dtos.SpeakerSummary) : Models.SpeakerSummary =
        { Id = dto.Id
          Forename = dto.Forename
          Surname = dto.Surname
          Rating = dto.Rating
          ImageUri = dto.ImageUri }

    let private convertToAdminSummary (dto : Dtos.AdminSummary) : Models.AdminSummary =
        { Id = dto.Id
          Forename = dto.Forename
          Surname = dto.Surname
          ImageUri = dto.ImageUri }

    let convertToLastContactSummary (dto : Dtos.LastContact) : Models.LastContactSummary =
        { Date = dto.Date; SenderId = dto.ProfileIdOne; ReceiverId = dto.ProfileIdTwo }


    let getLastContact (senderId : Guid) (receiverId : Guid) (lastContacts : Dtos.LastContact[]) =
        lastContacts
        |> Seq.tryFind (fun lastContact -> (lastContact.ProfileIdOne.Equals senderId && lastContact.ProfileIdTwo.Equals receiverId) || (lastContact.ProfileIdOne.Equals receiverId && lastContact.ProfileIdTwo.Equals senderId))
        |> Option.map convertToLastContactSummary

    let toModel (lastContacts : Dtos.LastContact[]) (session : Dtos.Session) : Models.Session =
        let spk = session.Speaker |> convertToSpeakerSummary
        let adm = session.Admin |> Option.map convertToAdminSummary
        let lc =
            match adm with 
            | Some admin -> getLastContact admin.Id spk.Id lastContacts
            | None -> None
        { Id = session.Id
          Title = session.Title
          Status = session.Status
          Date = session.Date
          DateAdded = session.DateAdded
          Speaker = spk
          Admin = adm
          LastContact = lc }