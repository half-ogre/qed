namespace qed.AspNet

open Configuration
open Microsoft.Owin
open Owin

type OwinConfigurator() = 
    member x.Configuration (appBuilder: IAppBuilder) =
        appBuilder 
        |> useQed
        |> ignore

module Startup =
    [<assembly: OwinStartup(typeof<OwinConfigurator>)>]
    do()