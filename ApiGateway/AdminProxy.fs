module AdminProxy

open HandlesProxy
open ProfilesProxy
open DataTransform

let getAdmin adminId = 
    let profile = getProfile adminId
    let handlesDtos = getHandlesByProfileId adminId
    Profile.toAdmin handlesDtos profile

let getAdminSummary adminId = getProfile adminId |> Profile.toAdminSummary