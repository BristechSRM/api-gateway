module LogHttpMiddleware

open Serilog
open System
open System.Collections.Generic
open System.Threading.Tasks
open System.Web.Http

let awaitTask = Async.AwaitIAsyncResult >> Async.Ignore 

type LogHttpMiddleware(next: Func<IDictionary<string,obj>, Task>) =

  member this.Invoke (environment: IDictionary<string,obj>) : Task =
    async {

      let verb = environment.Item("owin.RequestMethod").ToString()
      let path = environment.Item("owin.RequestPath").ToString()
      Log.Debug("{0} {1}", verb, path)

      do!
        awaitTask <| next.Invoke environment

      let code = Convert.ToInt32(environment.Item("owin.ResponseStatusCode").ToString())
      Log.Information("{0} {1} {2}", verb, path, code)

    } |> Async.StartAsTask :> Task
