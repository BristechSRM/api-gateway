module Program

open Config
open Microsoft.Owin.Hosting
open Logging
open Serilog
open System
open System.Configuration
open System.Threading

(*
    Do not run Visual Studio as Administrator!

    Open a command prompt as Administrator and run the following command, replacing username with your username
    netsh http add urlacl url=http://*:8080/ user=username
*)
[<EntryPoint>]
let main _ =
    JsonSettings.setDefaults()
    setupLogging()

    try
        use server = WebApp.Start(baseUrl, StartupConfig.configure)
        Log.Information("Listening on {Address}", baseUrl)

        let waitIndefinitelyWithToken = 
            let cancelSource = new CancellationTokenSource()
            cancelSource.Token.WaitHandle.WaitOne() |> ignore
        0

    with
    | ex ->
        Log.Fatal("Exception: {0}", ex)
        1
