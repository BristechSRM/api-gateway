module ProfilesFacade

open HandlesProxy
open ProfilesProxy

let getProfileAndConvertWith conversionFunc profileId = 
    let profile = getProfile profileId
    let handlesDtos = getHandlesByProfileId profileId
    conversionFunc handlesDtos profile

let getProfilesAndConvertWith (getFunc : unit -> Dtos.Profile []) conversionFunc = 
    let profiles = getFunc()
    profiles |> Seq.sortBy(fun profile -> profile.Forename + profile.Surname) |> Seq.map (fun profile ->
        let handlesDtos = getHandlesByProfileId profile.Id
        conversionFunc handlesDtos profile)

let addProfileAndHandles (profile : Dtos.Profile) (handles : Dtos.Handle seq) = 
    let newProfileId = addProfile profile
    let handlesWithProfileId = handles |> Seq.map (fun handle -> {handle with ProfileId = newProfileId})
    handles |> Seq.iter postHandle
    newProfileId
