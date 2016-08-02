module AdminFacade

open ProfilesFacade
open DataTransform

let getAdmin adminId = getProfileAndConvertWith Profile.toAdmin adminId

let getAdmins () = getProfilesAndConvertWith ProfilesProxy.getAdminProfiles Profile.toAdmin
