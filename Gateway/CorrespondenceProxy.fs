module CorrespondenceProxy

open Config
open Dtos
open JsonHttpClient
open System

let getCorrespondence (senderId : string) (receiverId : string) = 
    let uri = Uri <| String.Format("{0}?profileIdOne={1}&profileIdTwo={2}", correspondenceUrl, senderId, receiverId)
    get<CorrespondenceItem []>(uri)