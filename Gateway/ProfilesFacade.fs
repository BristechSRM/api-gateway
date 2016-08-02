module ProfilesFacade

open HandlesProxy
open ProfilesProxy

let getProfileAndConvertWith conversionFunc profileId = 
    let profile = getProfile profileId
    let handlesDtos = getHandlesByProfileId profileId
    conversionFunc handlesDtos profile

let getProfilesAndConvertWith (getFunc : unit -> Dtos.Profile []) conversionFunc = 
    let profiles = getFunc()
    profiles |> Seq.map (fun profile ->
        let handlesDtos = getHandlesByProfileId profile.Id
        conversionFunc handlesDtos profile)
