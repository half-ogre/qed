module Configuration

open Owin
open Microsoft.Owin

let configureQed (appBuilder: IAppBuilder) =
    appBuilder.Run(fun context -> 
        context.Response.StatusCode <- 200
        context.Response.WriteAsync "QED")