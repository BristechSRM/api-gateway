module StartupConfig

open Serilog
open Owin
open System.Web.Http
open IdentityServer3.AccessTokenValidation
open Bristech.Srm.HttpConfig
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
    let config = Default.config |> configureFilters
    app.UseWebApi(config) 

let configure (app : IAppBuilder) = 
    Log.Information("Performing Custom Auth configuration")
    app 
    |> configureBearerTokenAuth
    |> configureDataProtection
    |> configureWebApi
    |> ignore
