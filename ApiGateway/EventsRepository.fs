module EventsRepository

open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Serilog
open Dtos
open Models

let sessionsUri = "http://api.bris.tech/sessions/"

let convertToISO8601 (datetime : DateTime) =
    datetime.ToString("yyyy-MM-ddTHH\:mm\:ss\Z")

let getIds (sessions: SessionSummaryDto[]) =
    sessions
    |> Array.map (fun session -> session.Id)

let onSameDay (datetime1: DateTime) (datetime2: DateTime) =
    datetime1.Date = datetime2.Date

let convertToEventSession (session: SessionSummaryDto) =
    let startDate = Option.ofNullable session.Date
    let endDate =
        match startDate with
        | Some date -> Some <| date.AddHours(1.0)
        | None -> None
    { Id = session.Id
      Title = session.Title
      Description = "Et nulla id in minim laboris commodo Lorem ullamco. Ipsum commodo est aliquip duis sint. Adipisicing excepteur commodo deserunt laboris ut elit esse enim voluptate duis. Commodo veniam nulla excepteur mollit consectetur ullamco enim aute. Id eiusmod aute cillum voluptate deserunt velit aliquip dolor.\nQuis aliquip mollit ullamco culpa occaecat. Reprehenderit occaecat nulla et et. Enim Lorem magna tempor laborum qui laboris aute irure in aute non deserunt.\nAliqua magna non dolor aliqua dolore ex ut velit et deserunt. Sint enim ut id duis. Non sit mollit ex sint cupidatat excepteur enim labore sint incididunt enim mollit fugiat. Quis et excepteur culpa nostrud ex magna. Officia deserunt in consequat fugiat fugiat consectetur consequat culpa do consectetur consequat. Sit mollit dolore magna cupidatat aliquip deserunt nostrud voluptate duis officia. Do duis nisi sit consectetur enim eiusmod quis elit aliquip commodo proident non.\nEt Lorem Lorem reprehenderit nisi dolor magna. Id do irure ipsum nisi nisi esse cillum magna consectetur aliqua. Adipisicing nisi qui occaecat est. Ad minim labore magna mollit pariatur do. Enim Lorem anim do Lorem nisi laborum officia consequat qui officia sint sunt exercitation. Do voluptate quis voluptate consequat sit irure consequat aliquip dolor nulla. Aliqua laboris proident ullamco amet occaecat tempor sit labore.\nOfficia id non aliquip pariatur nisi mollit pariatur aliquip culpa ipsum. Duis ea eiusmod deserunt nostrud dolor excepteur dolore. Est commodo esse id sint exercitation eiusmod reprehenderit sit irure cillum amet. Do minim ad labore aliqua ut nostrud pariatur velit occaecat cupidatat ullamco. Nostrud aliqua sunt consectetur duis consequat elit culpa."
      SpeakerId = session.SpeakerId
      SpeakerForename = session.SpeakerForename
      SpeakerSurname = session.SpeakerSurname
      SpeakerBio = "Aute fugiat consequat minim laborum. Esse sunt excepteur esse consequat. Aliquip magna nostrud consectetur incididunt non id ut proident qui cillum eiusmod ad reprehenderit.\nNisi amet elit cillum nisi voluptate quis elit commodo eiusmod nisi tempor incididunt. Tempor in sit do qui id aliqua officia excepteur duis. Aliquip fugiat enim cillum pariatur duis ad id. Aliqua eu consequat anim veniam. Culpa labore elit labore non exercitation. Duis nisi dolore laboris sunt reprehenderit quis exercitation reprehenderit ex aliquip do do. Velit occaecat in enim eu irure ullamco.\nId laborum est quis veniam excepteur ea esse duis dolor aliqua. Duis pariatur est fugiat labore aliquip. Veniam commodo irure irure excepteur dolore excepteur officia adipisicing sit pariatur proident nostrud. Magna minim do occaecat ea eiusmod. Labore est deserunt laboris amet. Proident consectetur esse fugiat officia nostrud officia."
      SpeakerImageUri = session.SpeakerImageUrl
      SpeakerRating = session.SpeakerRating
      StartDate = startDate
      EndDate = endDate }

let getEvents() = 
    use client = new HttpClient()

    try
        let result = client.GetAsync(sessionsUri).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let sessionJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Sessions endpoint found")
            let sessions = JsonConvert.DeserializeObject<SessionSummaryDto[]>(sessionJson)
            let events =
                sessions
                |> Array.filter (fun session -> session.Date.HasValue)
                |> Array.groupBy (fun session -> session.Date.Value)
                |> Array.map (fun (date, eventSessions) -> {EventSummary.Id = date.Date |> convertToISO8601; Date = date; Description = ""; Location = ""; Sessions = getIds eventSessions})
            Success(events)
        | _ ->
            Log.Information("Status code: {statusCode}. Reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase)
            Failure { HttpStatusCode = result.StatusCode; Body = result.ReasonPhrase }
    with
        // Endpoint not found
        | :? AggregateException ->
            Log.Information("Could not reach sessions endpoint")
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "Could not reach sessions endpoint" }
        | ex ->
            Log.Information("Unhandled exception: {message}", ex.Message)
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred: " + ex.Message }

let getEvent(id) =
    use client = new HttpClient()

    try
        let date = DateTime.Parse(id, null, System.Globalization.DateTimeStyles.RoundtripKind)
        let result = client.GetAsync(sessionsUri).Result
        match result.StatusCode with
        | HttpStatusCode.OK ->
            let sessionJson = result.Content.ReadAsStringAsync().Result
            Log.Information("Sessions endpoint found")
            let sessions =
                JsonConvert.DeserializeObject<SessionSummaryDto[]>(sessionJson)
                |> Array.filter (fun session -> session.Date.HasValue && onSameDay session.Date.Value date)
                |> Array.map convertToEventSession
            let event = { Id = date.Date |> convertToISO8601; Date = date; Description = ""; Location = ""; Sessions = sessions }
            Success(event)
        | _ ->
            Log.Information("Status code: {statusCode}. Reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase)
            Failure { HttpStatusCode = result.StatusCode; Body = result.ReasonPhrase }
    with
        // Endpoint not found
        | :? AggregateException ->
            Log.Information("Could not reach sessions endpoint")
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "Could not reach sessions endpoint" }
        | :? FormatException ->
            Log.Information("Event not found")
            Failure { HttpStatusCode = HttpStatusCode.NotFound; Body = "Event not found" }
        | ex ->
            Log.Information("Unhandled exception: {message}", ex.Message)
            Failure { HttpStatusCode = HttpStatusCode.InternalServerError; Body = "An unhandled error occurred: " + ex.Message }