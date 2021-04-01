module Function

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.Extensions.Configuration

open FSharp.Json;
open Giraffe.ResponseWriters

type Person = { name: string; age: int }

let HandleFn(payload: string) (ctx: HttpContext) =
    task {
        let requestParameters = Json.deserialize<Person> payload
        return json $"Hello {requestParameters.name}, you are {requestParameters.age} years old."
    }
