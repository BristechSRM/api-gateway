module SpeakerRepository

open HandlesRepository
open ProfilesRepository
open DataTransform

let getSpeaker speakerId = 
    let profile = getProfile speakerId 
    let handlesDtos = getHandlesByProfileId speakerId 
    Profile.toSpeaker handlesDtos profile

let getSpeakerSummary speakerId = getProfile speakerId |> Profile.toSpeakerSummary
