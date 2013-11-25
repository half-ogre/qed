[<System.Runtime.CompilerServices.Extension>]
module OwinExtensions
    open System
    open System.IO
    open System.Threading
    open System.Threading.Tasks
    open System.Collections.Generic
    open System.Runtime.CompilerServices
    open Constants.Owin

    type Environment = IDictionary<string, Object>
    type Form = IDictionary<string, List<string>>
    type Headers = IDictionary<string, string[]>

    [<Literal>]
    let RequestFormKey = "owinextensions.RequestForm"

    [<Extension>]
    let Get<'a> (environment:Environment) key =
        match environment.TryGetValue(key) with
            | true, value -> value :?> 'a
            | _, _ -> Unchecked.defaultof<'a>

    [<Extension>]
    let GetCallCancelled environment =
        Get<System.Threading.CancellationToken> environment CallCancelledKey

    [<Extension>]
    let GetMethod environment =
        Get<string> environment RequestMethodKey

    [<Extension>]
    let GetPath environment =
        Get<string> environment RequestPathKey

    [<Extension>]
    let GetRequestHeaders environment = 
        Get<Headers> environment RequestHeadersKey

    [<Extension>]
    let GetResponseBody environment =
        Get<Stream> environment ResponseBodyKey

    [<Extension>]
    let GetResponseHeaders environment =
        Get<Headers> environment ResponseHeadersKey

    let normalize (item:string) = Uri.UnescapeDataString(item.Replace('+', ' '))

    let ParseForm formText =
        let form = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase) :> Form 

        if String.IsNullOrEmpty(formText) then
            form
        else
            let parse (s:string) =
                s.Split([|'&'|])
                |> Array.map (fun pair -> pair.Split([|'='|]))
                |> Array.map (fun pair -> pair |> Array.map normalize)
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
    let ReadFormAsync environment : Task<Form> =

        let form = Get<Form> environment RequestFormKey

        let createForm =
            async {
                use streamReader = new StreamReader (GetResponseBody environment) 
                let! formText = Async.AwaitTask(streamReader.ReadToEndAsync())
                let form = ParseForm formText
                environment.[RequestFormKey] <- form
                return form
                }
            |> Async.StartAsTask // TODO: is this spawning unnecessary threads? 

        match form with
            | null -> createForm 
            | _ -> Task.FromResult form

    [<Extension>]
    let WriteAsync environment buffer offset count (cancel:CancellationToken) =

        if environment = null then invalidArg "environment" "environment is required"
        if buffer = null then invalidArg "buffer" "buffer is required"

        if cancel.IsCancellationRequested then
            let tcs = new TaskCompletionSource<Object>()
            tcs.TrySetCanceled() |> ignore
            tcs.Task :> Task
        else
            let body = GetResponseBody environment
            if body = null then
                raise (InvalidOperationException("The OWIN response body stream is missing from the environment."))
            else
                body.WriteAsync(buffer, offset, count, cancel)

    [<Extension>]
    let WriteAsync2 environment (text:string) (encoding:System.Text.Encoding) cancel =
        if environment = null then invalidArg "environment" "environment is required"
        if text = null then invalidArg "text" "text is required"
        if encoding = null then invalidArg "encoding" "encoding is required"

        let buffer = encoding.GetBytes(text)

        WriteAsync environment buffer 0 buffer.Length cancel

    [<Extension>]
    let WriteAsync3 environment (text:string) = 
        if environment = null then invalidArg "environment" "environment is required"
        if text = null then invalidArg "text" "text is required"

        WriteAsync2 environment text System.Text.Encoding.UTF8 (GetCallCancelled environment)

    [<Extension>]
    let SetStatusCode (environment:Environment) (statusCode:int) = 
        environment.[ResponseStatusCodeKey] <- statusCode
