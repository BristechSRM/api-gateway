module ProfilesProxy

open Config
open Dtos
open JsonHttpClient
open RestModels
open System

let getProfiles() = get<Profile []> profilesUri

let getProfile (pid : Guid) = get<Profile> <| new Uri(profilesUri, pid.ToString())

let getAdminProfiles() = get<Profile []> <| new Uri(profilesUri, "?isAdmin=true")

let patchProfile (pid : Guid) (op : PatchOp) = patch profilesUri pid op
