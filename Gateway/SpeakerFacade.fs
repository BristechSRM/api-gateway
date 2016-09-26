module SpeakerFacade

open System
open ProfilesFacade
open DataTransform

let getSpeaker speakerId = getProfileAndConvertWith Profile.toSpeaker speakerId

let getSpeakers () = getProfilesAndConvertWith ProfilesProxy.getProfiles Profile.toSpeaker

let addSpeaker (speaker : Models.Speaker) = 
    let profile = Profile.fromSpeaker speaker
    let handles = 
        if box speaker.Handles |> isNull 
            then Seq.empty 
        else 
            speaker.Handles |> Seq.map (Handle.toHandleDto Guid.Empty)
    addProfileAndHandles profile handles