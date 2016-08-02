module SpeakerFacade

open ProfilesFacade
open DataTransform

let getSpeaker speakerId = getProfileAndConvertWith Profile.toSpeaker speakerId

let getSpeakers () = getProfilesAndConvertWith ProfilesProxy.getProfiles Profile.toSpeaker
