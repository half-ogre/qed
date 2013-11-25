[<System.Runtime.CompilerServices.Extension>]
module OwinExtensions
    open System
    open System.Collections.Generic
    open System.Runtime.CompilerServices
    open Constants.Owin

    type dict = IDictionary<string, Object>

    [<Extension>]
    let Get<'a> (environment:IDictionary<string, System.Object>) key =
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
        Get<IDictionary<string, string[]>> environment RequestHeadersKey

    [<Extension>]
    let GetResponseBody environment =
        Get<System.IO.Stream> environment ResponseBodyKey

    [<Extension>]
    let GetResponseHeaders environment =
        Get<IDictionary<string, string[]>> environment ResponseHeadersKey

    let normalize (item:string) = Uri.UnescapeDataString(item.Trim().Replace('+', ' '))

    let parseForm formText =
        let form = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)

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
    let SetStatusCode (environment:dict) (statusCode:int) = 
        environment.[ResponseStatusCodeKey] <- statusCode
