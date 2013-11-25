// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Constants.fs"
#load "OwinExtensions.fs"
open OwinExtensions
open System
open System.Collections.Generic

let dd = parseForm "a=b&x=123&x=234"