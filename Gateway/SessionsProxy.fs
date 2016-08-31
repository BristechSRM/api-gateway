module SessionsProxy

open Config
open Dtos
open JsonHttpClient
open RestModels
open System

let getSessions() = get<Session []> sessionsUri

let getSession (sid : Guid) = get<Session> <| new Uri(sessionsUri, sid.ToString())

let getSessionsByEventId (eventId : Guid) = get<Session []> <| new Uri(sessionsUri, "?eventId=" + eventId.ToString())

let addSession (session : Session) = postAndGetGuid sessionsUri session

let patchSession (sid : Guid) (op : PatchOp) = patch sessionsUri sid op
