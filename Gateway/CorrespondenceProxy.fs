module CorrespondenceProxy

open Config
open Dtos
open JsonHttpClient
open System

let getCorrespondence (senderId : string) (receiverId : string) = get<CorrespondenceItem []> <| new Uri(correspondenceUri, sprintf "?profileIdOne=%s&profileIdTwo=%s" senderId receiverId)
