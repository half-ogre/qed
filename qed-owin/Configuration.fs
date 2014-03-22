module Configuration

open DispatcherMiddleware
open Owin
open OwinEnvironment
open Microsoft.Owin

let useQed (appBuilder: IAppBuilder) =
    appBuilder
    |> useDispatcher(fun dispatcher ->
        dispatcher
        |> get "/" None (fun (environment, routeParams, next) ->
            environment
            |> setStatusCode 200
            |> writeAsync (Text "QED")))