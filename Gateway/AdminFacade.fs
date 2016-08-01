module AdminFacade

open HandlesProxy
open ProfilesProxy
open DataTransform

let getAdmin adminId = 
    let profile = getProfile adminId
    let handlesDtos = getHandlesByProfileId adminId
    Profile.toAdmin handlesDtos profile

let getAdmins () = 
    let profiles = getAdminProfiles()
    profiles |> Seq.map (fun profile -> 
        let handlesDtos = getHandlesByProfileId profile.Id
        Profile.toAdmin handlesDtos profile)
