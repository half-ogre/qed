module OwinEnvironment

open Owin
open System.IO
open System.Text

type WriteSource = 
    | Text of string
    | Bytes of byte[]

let private get<'a> key (environment:OwinEnvironment) =
    match environment.TryGetValue(key) with
        | true, value -> Some (value :?> 'a)
        | false, value -> None

let private set key value (environment:OwinEnvironment) =
    environment.[key] <- value
    environment

let getRequestMethod environment = 
    match environment |> get<string> "owin.RequestMethod" with
    | None -> failwith "The OWIN environment does not contain the required key \"owin.RequestMethod\"."
    | Some v -> v

let getRequestPath environment = 
    match environment |> get<string> "owin.RequestPath" with
    | None -> failwith "The OWIN environment does not contain the required key \"owin.RequestPath\"."
    | Some v -> v
    
let getResponseBody environment = 
    match environment |> get<Stream> "owin.ResponseBody" with
    | None -> failwith "The OWIN environment does not contain the required key \"owin.ResponseBody\"."
    | Some v -> v

let setStatusCode statusCode environment =
    environment
    |> set "owin.ResponseStatusCode" statusCode

let writeAsync source environment =
    let bytes = 
        match source with
        | Text s -> Encoding.UTF8.GetBytes s
        | Bytes b -> b
    let body = environment|> getResponseBody
    body.WriteAsync(bytes, 0, bytes.Length)