module SpeakerFacade

open HandlesProxy
open ProfilesProxy
open DataTransform

let getSpeaker speakerId = 
    let profile = getProfile speakerId 
    let handlesDtos = getHandlesByProfileId speakerId 
    Profile.toSpeaker handlesDtos profile

let getSpeakers () = 
    let profiles = getProfiles() 
    profiles |> Seq.map (fun profile -> 
        let handlesDtos = getHandlesByProfileId profile.Id 
        Profile.toSpeaker handlesDtos profile)
