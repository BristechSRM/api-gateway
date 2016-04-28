module StartupConfig

open System.Web.Http

let configureFilters (config : HttpConfiguration) = 
    config.Filters.Add(new AuthorizeAttribute())
    config
