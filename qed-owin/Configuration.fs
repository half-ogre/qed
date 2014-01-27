module Configuration

open Owin
open Microsoft.Owin

let useQed (appBuilder: IAppBuilder) =
    appBuilder.Run(fun context -> 
        context.Response.StatusCode <- 200
        context.Response.WriteAsync "QED")
    appBuilder