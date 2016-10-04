module NotesProxy

open Config
open Dtos
open JsonHttpClient
open System
open RestModels

let getNotesBySessionId (sessionId : Guid) = get<Note []> <| new Uri(notesUri, "?sessionId=" + sessionId.ToString())

let postNote (note : Note) = postAndGetGuid notesUri note

let patchNote (nid : Guid) (op : PatchOp) = patch notesUri nid op
