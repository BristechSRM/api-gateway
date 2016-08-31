module Config
open System
open System.Configuration

let getConfigValue (key : string) = 
    let value = ConfigurationManager.AppSettings.Item(key)
    if String.IsNullOrWhiteSpace value then
        failwith <| sprintf "Missing configuration value: %s" key
    else 
        value

let getUriConfigValue (key : string) = getConfigValue key |> Uri

let baseUrl = getConfigValue "BaseUrl"
let sessionsServiceUri = getUriConfigValue "SessionsServiceUrl"
let commsServiceUri = getUriConfigValue "CommsServiceUrl"

let handlesUri = Uri (sessionsServiceUri, "handles")
let sessionsUri = Uri (sessionsServiceUri, "sessions")
let sessionIdsUri = Uri (sessionsServiceUri, "sessions/ids")
let profilesUri = Uri (sessionsServiceUri, "profiles")
let eventsUri = Uri (sessionsServiceUri, "events")
let lastContactUri = Uri (commsServiceUri, "last-contact")
let correspondenceUri = Uri (commsServiceUri, "correspondence")
