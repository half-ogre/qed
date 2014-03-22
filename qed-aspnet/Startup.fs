namespace qed.AspNet

open Configuration
open Microsoft.Owin
open Owin

type Startup() = 
    member x.Configuration (appBuilder: IAppBuilder) =
        appBuilder 
        |> useQed
        |> ignore

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Startup =
    [<assembly: OwinStartup(typeof<Startup>)>]
    do()