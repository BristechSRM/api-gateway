﻿module AdminRepository

open HandlesRepository
open ProfilesRepository
open DataTransform

let getAdmin adminId = 
    let profile = getProfile adminId
    let handlesDtos = getHandlesByProfileId adminId
    Profile.toAdmin handlesDtos profile