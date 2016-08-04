module SessionsProxy

open Config
open Dtos
open JsonHttpClient
open RestModels
open System

let getSessions() = get<Session []>(new Uri(sessionsUrl))

let getSession (sid : Guid) = get<Session>(new Uri(sessionsUrl + sid.ToString()))

let addSession (session : Session) = postAndGetGuid (new Uri(sessionsUrl)) session

let patchSession (sid : Guid) (op : PatchOp) = patch (new Uri(sessionsUrl)) sid op
