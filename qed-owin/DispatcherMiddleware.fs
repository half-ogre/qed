[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module DispatcherMiddleware

open Owin
open OwinEnvironment
open System
open System.Collections.Generic
open System.Text.RegularExpressions
open System.Threading.Tasks

type RouteMiddlewares = OwinMiddleware list option

type RouteParams = Map<string, string>

type RouteHandler = OwinEnvironment * RouteParams * OwinApplication -> Task

type Route = { Url: Regex; Middlewares: RouteMiddlewares; Handler: RouteHandler }

type Dispatcher(routeMap:Map<string, Route list>) = 
    
    let urlTokenRegex = new Regex(@"\{([a-z]+)\}", RegexOptions.IgnoreCase)

    let routeUrl url  =
        new Regex("^" + urlTokenRegex.Replace(url, @"(?<$1>[^/]+)") + "$", RegexOptions.IgnoreCase)
    
    let addRoute requestMethod url middlewares handler =
        new Dispatcher(
            match routeMap |>   Map.tryFind requestMethod with
            | None -> routeMap |> Map.add requestMethod List.empty<Route>
            | Some routeList -> routeMap
            |> Map.map (fun key value -> 
                match key with
                | k when k = requestMethod -> value @ [ { Url = url |> routeUrl; Middlewares = middlewares; Handler = handler } ]
                | _ -> value))

    member self.Find (requestMethod: string) (requestPath: string) =
        let rec findRoute routeList = 
            match routeList with 
                | [] -> None, None
                | h::t -> 
                    let matches = h.Url.Match(requestPath)
                    if matches.Success then 
                        Some h, matches |> (fun matches -> 
                            h.Url.GetGroupNames() 
                            |> Array.toList 
                            |> List.map (fun v -> (v, matches.Groups.[v].Value)) 
                            |> Map.ofList 
                            |> Some)
                    else 
                        findRoute t
        match routeMap |> Map.tryFind requestMethod with
        | None -> None, None
        | Some routeList -> routeList |> findRoute

    member self.Get (url: string) (middlewares: RouteMiddlewares) (handler: RouteHandler) = 
         addRoute "get" url middlewares handler

type DispatcherMiddleware() =
    static member Create (configure: Dispatcher -> Dispatcher) =
        let dispatcher = configure (new Dispatcher(Map.empty<string, Route list>))
        fun (next:OwinApplication) -> 
            fun (environment:OwinEnvironment) ->
                let rec toOwinApplication (chain:OwinMiddleware list) =
                    match chain with
                    | [] -> next
                    | h::t -> new OwinApplication(fun env ->
                        h.Invoke(t |> toOwinApplication).Invoke(env))
                let add (handler:RouteHandler) (middlewares:RouteMiddlewares) =
                    let handler = new OwinMiddleware(fun (handlerNext:OwinApplication) -> 
                        new OwinApplication(fun (handlerEnvironment:OwinEnvironment) ->
                            handler (handlerEnvironment, Map.empty<string, string>, handlerNext)))
                    match middlewares with
                    | None -> [ handler ]
                    | Some v -> v @ [ handler ]
                let requestMethod = (environment |> getRequestMethod).ToLower()
                let requestPath = environment |> getRequestPath
                let route, routeParams = dispatcher.Find requestMethod requestPath
                match route with
                | None -> next.Invoke(environment)
                | Some r -> (r.Middlewares |> add r.Handler |> toOwinApplication).Invoke(environment)

let get (url: string) (middlewares: RouteMiddlewares) (handler: RouteHandler) (dispatcher: Dispatcher) =
    dispatcher.Get url middlewares handler
    
let useDispatcher (configure: Dispatcher -> Dispatcher) (appBuilder: IAppBuilder) =
    appBuilder.Use(DispatcherMiddleware.Create(configure))