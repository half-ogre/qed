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

        let parseForm formText =
            let form = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase) :> Form 

            if String.IsNullOrEmpty(formText) then
                form
            else
                let parse (s:string) =
                    s.Split([|'&'|])
                    |> Array.map (fun pair -> pair.Split([|'='|]))
                    |> Array.map (Array.map normalize)
                    |> Array.map (fun pair -> pair.[0], pair.[1])
                    |> Array.toList
                
                let rec populateDictionary list =
                    match list with
                    | [] -> form
                    | (name,value)::tail ->
                        match form.TryGetValue(name) with
                            | false, _ -> form.Add(name, new List<string>([|value|])); populateDictionary tail
                            | _, existing -> existing.Add value; populateDictionary tail

                parse formText |> populateDictionary

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
        static member GetRequestHeaders (environment) = 
            EnvironmentExt.Get<Headers> (environment, RequestHeadersKey)

        [<Extension>]
        static member GetResponseBody (environment) =
            EnvironmentExt.Get<Stream> (environment, ResponseBodyKey)

        [<Extension>]
        static member GetResponseHeaders (environment) =
            EnvironmentExt.Get<Headers> (environment, ResponseHeadersKey)

        [<Extension>]
        static member ReadFormAsync (environment) : Task<Form> =

            let form = EnvironmentExt.Get<Form> (environment, RequestFormKey)

            let createForm =
                async {
                    use streamReader = new StreamReader (EnvironmentExt.GetResponseBody environment) 
                    let! formText = Async.AwaitTask(streamReader.ReadToEndAsync())
                    let form = helpers.parseForm formText
                    environment.[RequestFormKey] <- form
                    return form
                    }
                |> Async.StartAsTask // TODO: is this spawning unnecessary threads? 

            match form with
                | null -> createForm 
                | _ -> Task.FromResult form

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