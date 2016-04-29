module StartupConfig

open Owin
open System.Web.Http
open IdentityServer3.AccessTokenValidation
open Bristech.Srm.HttpConfig
open System.Configuration

let authServiceUri = ConfigurationManager.AppSettings.Item("AuthServiceUri")

let configureFilters (config : HttpConfiguration) = 
    config.Filters.Add(new AuthorizeAttribute())
    config

let configureBearerTokenAuth (app : IAppBuilder) =
    app.UseIdentityServerBearerTokenAuthentication
        (new IdentityServerBearerTokenAuthenticationOptions(Authority = authServiceUri, RequiredScopes = [| "api" |]))

let configureWebApi (app : IAppBuilder) = 
    let config = Default.config |> configureFilters
    app.UseWebApi(config) 

let configure (app : IAppBuilder) = 
    app 
    |> configureBearerTokenAuth
    |> configureWebApi
    |> ignore