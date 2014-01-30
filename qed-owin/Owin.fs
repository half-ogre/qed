module Owin

open System
open System.Collections.Generic
open System.Threading.Tasks

type OwinEnvironment = IDictionary<string, Object>

type OwinApplication = Func<OwinEnvironment, Task>

type OwinMiddleware = Func<OwinApplication, OwinApplication>


