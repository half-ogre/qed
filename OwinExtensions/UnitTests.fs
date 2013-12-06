module UnitTests.``the parseForm function``
#if DEBUG

open System
open System.Linq
open OwinExtensions
open helpers
open Xunit

[<Fact>] 
let ``parses a single key and value`` () = 
    let formText = "key=value"

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("key", form.Keys.ElementAt(0))
    Assert.Equal("value", form.Values.ElementAt(0).ElementAt(0))

[<Fact>] 
let ``parses a key with multiple values`` () =
    let  formText = "key=value1&key=value2"

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("key", form.Keys.ElementAt(0))
    Assert.Equal("value1", form.Values.ElementAt(0).ElementAt(0))
    Assert.Equal("value2", form.Values.ElementAt(0).ElementAt(1))

[<Fact>] 
let ``parses form text with multuple keys and values`` () =
    let formText = "key1=value1a&key1=value1b&key2=value2"

    let form = parseForm formText

    Assert.Equal(2, form.Count)
    Assert.Equal("key1", form.Keys.ElementAt(0))
    Assert.Equal("key2", form.Keys.ElementAt(1))
    Assert.Equal("value1a", form.Values.ElementAt(0).ElementAt(0))
    Assert.Equal("value1b", form.Values.ElementAt(0).ElementAt(1))
    Assert.Equal("value2", form.Values.ElementAt(1).ElementAt(0))

[<Fact>] 
let ``decodes keys`` () =
    let formText = sprintf "%s=value" (Uri.EscapeDataString("&key"))

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("&key", form.Keys.ElementAt(0))

[<Fact>] 
let ``decodes keys with spaces`` () =
    let formText = "key+one=value"

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("key one", form.Keys.ElementAt(0))

[<Fact>] 
let ``decodes values`` () =
    let formText = sprintf "key=%s" (Uri.EscapeDataString("&value"))

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("&value", form.Values.ElementAt(0).ElementAt(0))

[<Fact>] 
let ``decodes values with spaces`` () =
    let formText = "key=value+one"

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("value one", form.Values.ElementAt(0).ElementAt(0))

[<Fact>] 
let ``parses a key with no value`` () =
    let  formText = "key="

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("key", form.Keys.ElementAt(0))
    Assert.Equal("", form.Values.ElementAt(0).ElementAt(0))

#endif