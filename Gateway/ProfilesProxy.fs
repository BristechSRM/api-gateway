module ProfilesProxy

open Config
open Dtos
open JsonHttpClient
open RestModels
open System

let getProfiles() = get<Profile []> (new Uri(profilesUrl))

let getProfile (pid : Guid) = get<Profile> (new Uri(profilesUrl + pid.ToString()))

let getAdminProfiles() = get<Profile []> (new Uri(profilesUrl + "?isAdmin=true"))

let patchProfile (pid : Guid) (op : PatchOp) = patch (new Uri(profilesUrl)) pid op
