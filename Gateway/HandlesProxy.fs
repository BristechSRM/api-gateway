module HandlesProxy

open Config
open Dtos
open JsonHttpClient
open RestModels
open System

let getHandlesByProfileId (pid : Guid) = get<Handle []> <| new Uri(handlesUri, "?profileId=" + pid.ToString())

let getHandle (hid : int) = get<Handle> <| new Uri(handlesUri, hid.ToString())

let patchHandle (hid : int) (op : PatchOp) = patch handlesUri hid op
