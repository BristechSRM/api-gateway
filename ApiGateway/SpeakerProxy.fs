module SpeakerProxy

open HandlesProxy
open ProfilesProxy
open DataTransform

let getSpeaker speakerId = 
    let profile = getProfile speakerId 
    let handlesDtos = getHandlesByProfileId speakerId 
    Profile.toSpeaker handlesDtos profile
