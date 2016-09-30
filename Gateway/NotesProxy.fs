module NotesProxy

open Config
open Dtos
open JsonHttpClient
open System

let getNotesBySessionId (sessionId : Guid) = get<Note []> <| new Uri(notesUri, "?sessionId=" + sessionId.ToString())
