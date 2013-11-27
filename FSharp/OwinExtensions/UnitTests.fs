#if DEBUG
module UnitTests.the_parseForm_function

open System
open System.Linq
open OwinExtensions
open helpers
open Xunit

[<Fact>] 
let parses_a_single_key_and_value() = 
    let formText = "key=value"

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("key", form.Keys.ElementAt(0))
    Assert.Equal("value", form.Values.ElementAt(0).ElementAt(0))

[<Fact>] 
let parses_a_key_with_multiple_values() =
    let  formText = "key=value1&key=value2"

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("key", form.Keys.ElementAt(0))
    Assert.Equal("value1", form.Values.ElementAt(0).ElementAt(0))
    Assert.Equal("value2", form.Values.ElementAt(0).ElementAt(1))

[<Fact>] 
let parses_form_text_with_multuple_keys_and_values() =
    let formText = "key1=value1a&key1=value1b&key2=value2"

    let form = parseForm formText

    Assert.Equal(2, form.Count)
    Assert.Equal("key1", form.Keys.ElementAt(0))
    Assert.Equal("key2", form.Keys.ElementAt(1))
    Assert.Equal("value1a", form.Values.ElementAt(0).ElementAt(0))
    Assert.Equal("value1b", form.Values.ElementAt(0).ElementAt(1))
    Assert.Equal("value2", form.Values.ElementAt(1).ElementAt(0))

[<Fact>] 
let decodes_keys() =
    let formText = sprintf "%s=value" (Uri.EscapeDataString("&key"))

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("&key", form.Keys.ElementAt(0))

[<Fact>] 
let decodes_keys_with_spaces() =
    let formText = "key+one=value"

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("key one", form.Keys.ElementAt(0))

[<Fact>] 
let decodes_values() =
    let formText = sprintf "key=%s" (Uri.EscapeDataString("&value"))

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("&value", form.Values.ElementAt(0).ElementAt(0))

[<Fact>] 
let decodes_values_with_spaces() =
    let formText = "key=value+one"

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("value one", form.Values.ElementAt(0).ElementAt(0))

[<Fact>] 
let parses_a_key_with_no_value() =
    let  formText = "key="

    let form = parseForm formText

    Assert.Equal(1, form.Count)
    Assert.Equal("key", form.Keys.ElementAt(0))
    Assert.Equal("", form.Values.ElementAt(0).ElementAt(0))

#endif