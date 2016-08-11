module Config
open System
open System.Configuration

let getConfigValue (key : string) = 
    let value = ConfigurationManager.AppSettings.Item(key)
    if String.IsNullOrWhiteSpace value then
        failwith <| sprintf "Missing configuration value: %s" key
    else 
        value

let getUriConfigValue (key : string) = 
    getConfigValue key |> Uri

let baseUrl = getConfigValue "BaseUrl"
let handlesUri = getUriConfigValue "HandlesUrl"
let sessionsUri = getUriConfigValue "SessionsUrl"
let profilesUri = getUriConfigValue "ProfilesUrl"
let eventsUri = getUriConfigValue "EventsUrl"
let lastContactUri = getUriConfigValue "LastContactUrl"
let correspondenceUri = getUriConfigValue "CorrespondenceUrl"
