namespace qed.AspNet

open Configuration
open Microsoft.Owin
open Owin

type OwinConfigurator() = 
    member x.Configuration (appBuilder: IAppBuilder) =
        configureQed appBuilder

module Startup =
    [<assembly: OwinStartup(typeof<OwinConfigurator>)>]
    do()