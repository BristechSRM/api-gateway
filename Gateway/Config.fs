module Config
open System
open System.Configuration

let getString (key : string) = 
    let value = ConfigurationManager.AppSettings.Item(key)
    if String.IsNullOrWhiteSpace value then
        failwith <| sprintf "Missing configuration value: %s" key
    else 
        value

let getUri (key : string) = getString key |> Uri

let baseUrl = getString "BaseUrl"
let sessionsServiceUri = getUri "SessionsServiceUrl"
let commsServiceUri = getUri "CommsServiceUrl"
let publishServiceUri = getUri "PublishServiceUrl"

//Note : when using the Uri constructor, slash must be on the end of a relative url that will have other parts attached
//Alternatively, use UriBuilder if things get more complicated. 
let handlesUri = Uri (sessionsServiceUri, "handles/")
let sessionsUri = Uri (sessionsServiceUri, "sessions/")
let sessionIdsUri = Uri (sessionsServiceUri, "sessions/ids/")
let profilesUri = Uri (sessionsServiceUri, "profiles/")
let eventsUri = Uri (sessionsServiceUri, "events/")
let meetupEventsUri = Uri (sessionsServiceUri, "meetupevents/")
let lastContactUri = Uri (commsServiceUri, "last-contact/")
let correspondenceUri = Uri (commsServiceUri, "correspondence/")
let publishUri = Uri(publishServiceUri, "publish/")
