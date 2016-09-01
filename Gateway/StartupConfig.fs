module StartupConfig

open Common
open LogHttpMiddleware
open LogExceptionHandler
open Serilog
open Owin
open System.Web.Http
open System.Web.Http.ExceptionHandling
open IdentityServer3.AccessTokenValidation
open System.Configuration
open Owin.Security.AesDataProtectorProvider

let authServiceUri = ConfigurationManager.AppSettings.Item("AuthServiceUrl")

let configureFilters (config : HttpConfiguration) = 
    config.Filters.Add(new AuthorizeAttribute())
    config

let configureBearerTokenAuth (app : IAppBuilder) =
    app.UseIdentityServerBearerTokenAuthentication
        (new IdentityServerBearerTokenAuthenticationOptions(Authority = authServiceUri, RequiredScopes = [| "api" |]))    

let configureDataProtection (app : IAppBuilder) =
    app.UseAesDataProtectorProvider("key")
    app

let configureWebApi (app : IAppBuilder) =
    app.Use LogHttpMiddleware |> ignore

    let config =
      new HttpConfiguration()
      |> Logging.configure
      |> Cors.configure
      |> Routes.configure
      |> Serialization.configure
      |> configureFilters

    config.Services.Replace(typedefof<IExceptionHandler>, new LogExceptionHandler())

    app.UseWebApi(config) 

let configure (app : IAppBuilder) = 
    Log.Information("Configured to require bearer token")
    app 
    |> configureBearerTokenAuth
    |> configureDataProtection
    |> configureWebApi
    |> ignore
