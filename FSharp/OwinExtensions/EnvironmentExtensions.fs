namespace OwinExtensions

open System
open System.IO
open System.Text
open System.Threading
open System.Threading.Tasks
open System.Collections.Generic
open System.Runtime.CompilerServices
open Constants

type Environment = IDictionary<string, Object>
type Form = IDictionary<string, List<string>>
type Headers = IDictionary<string, string[]>

module internal helpers =
    let private normalize (item:string) = Uri.UnescapeDataString(item.Replace('+', ' '))

    let private parse (s:string) =
        s.Split([|'&'|])
        |> Array.map (fun pair -> pair.Split([|'='|]))
        |> Array.map (Array.map normalize)
        |> Array.map (fun pair -> pair.[0], pair.[1])
        |> Array.toList

    let parseForm formText =
        let form = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase) :> Form 

        if not (String.IsNullOrEmpty formText) then
                
            for (name,value) in parse formText do
                match form.TryGetValue(name) with
                    | false, _ -> form.Add(name, new List<string>([|value|]))
                    | _, existing -> existing.Add value

        form

[<Extension>]
[<AbstractClass; Sealed>]
type EnvironmentExt() =

    [<Extension>]
    static member Get<'a> (environment:Environment, key) =
        match environment.TryGetValue(key) with
            | true, value -> value :?> 'a
            | _, _ -> Unchecked.defaultof<'a>

    [<Extension>]
    static member GetCallCancelled (environment) =
        EnvironmentExt.Get<CancellationToken> (environment, CallCancelledKey)

    [<Extension>]
    static member GetMethod (environment) =
        EnvironmentExt.Get<string> (environment, RequestMethodKey)

    [<Extension>]
    static member GetPath (environment) =
        EnvironmentExt.Get<string> (environment, RequestPathKey)

    [<Extension>]
    static member GetRequestBody (environment) =
        EnvironmentExt.Get<Stream> (environment, RequestBodyKey)

    [<Extension>]
    static member GetRequestHeaders (environment) = 
        EnvironmentExt.Get<Headers> (environment, RequestHeadersKey)

    [<Extension>]
    static member GetResponseBody (environment) =
        EnvironmentExt.Get<Stream> (environment, ResponseBodyKey)

    [<Extension>]
    static member GetResponseHeaders (environment) =
        EnvironmentExt.Get<Headers> (environment, ResponseHeadersKey)

    [<Extension>]
    static member ReadForm environment : Form =
        let form = EnvironmentExt.Get<Form>(environment, RequestFormKey)

        let createForm =
            let stream = EnvironmentExt.GetResponseBody(environment)
            use streamReader = new StreamReader(stream)
            let formText = streamReader.ReadToEnd()
            let form = helpers.parseForm(formText)
            environment.[RequestFormKey] <- form
            form

        match form with
            | null -> createForm 
            | _ -> form

    [<Extension>]
    static member WriteAsync (environment, buffer, offset, count, cancel:CancellationToken) =
        if environment = null then invalidArg "environment" "environment is required"
        if buffer = null then invalidArg "buffer" "buffer is required"

        if cancel.IsCancellationRequested then
            let tcs = new TaskCompletionSource<Object>()
            tcs.TrySetCanceled() |> ignore
            tcs.Task :> Task
        else
            match EnvironmentExt.GetResponseBody environment with
                | null -> raise (InvalidOperationException("The OWIN response body stream is missing from the environment."))
                | body -> body.WriteAsync(buffer, offset, count, cancel)

    [<Extension>]
    static member WriteAsync (environment, text:string, encoding:Encoding, cancel) =
        if environment = null then invalidArg "environment" "environment is required"
        if text = null then invalidArg "text" "text is required"
        if encoding = null then invalidArg "encoding" "encoding is required"

        let buffer = encoding.GetBytes(text)
        EnvironmentExt.WriteAsync (environment, buffer, 0, buffer.Length, cancel)

    [<Extension>]
    static member WriteAsync (environment, text) = 
        if environment = null then invalidArg "environment" "environment is required"
        if text = null then invalidArg "text" "text is required"

        let token = EnvironmentExt.GetCallCancelled environment
        EnvironmentExt.WriteAsync (environment, text, Encoding.UTF8, token)

    [<Extension>]
    static member SetStatusCode (environment:Environment, statusCode:int) = 
        environment.[ResponseStatusCodeKey] <- statusCode