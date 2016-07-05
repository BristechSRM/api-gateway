module DataTransform

module Handle = 
    let handleDtoToHandle (dto : Dtos.HandleDto) : Models.Handle =
        { Type = dto.Type
          Identifier = dto.Identifier }

module Profile = 

    let toAdmin (handles : Dtos.HandleDto seq) (profile : Dtos.Profile): Models.Admin =
        { Id = profile.Id
          Forename = profile.Forename
          Surname = profile.Surname
          ImageUri = profile.ImageUrl
          Handles = handles |> Seq.map Handle.handleDtoToHandle }

    let toSpeaker (handles : Dtos.HandleDto seq) (profile : Dtos.Profile) : Models.Speaker =
        { Id = profile.Id
          Forename = profile.Forename
          Surname = profile.Surname
          Rating = profile.Rating
          ImageUri = profile.ImageUrl
          Bio = profile.Bio
          Handles = handles |> Seq.map Handle.handleDtoToHandle }