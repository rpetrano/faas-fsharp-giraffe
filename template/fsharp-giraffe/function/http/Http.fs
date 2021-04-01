module FunctionHttp

open System
open FSharp.Data
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Giraffe
open System.IO
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Hosting
open Function

let Post (next: HttpFunc) (ctx: HttpContext) =
    task {
        let reader = new StreamReader(ctx.Request.Body)
        let! payload = reader.ReadToEndAsync()

        let! result = HandleFn payload ctx
        return! result next ctx
    }

let NotHandled (next: HttpFunc) (ctx: HttpContext) =
    setStatusCode HttpStatusCodes.MethodNotAllowed next ctx

let Handler (next: HttpFunc) (ctx: HttpContext) =
    match ctx.Request.Method with
    | "GET" -> NotHandled next ctx
    | "POST" -> Post next ctx
    | "PUT" -> NotHandled next ctx
    | "DELETE" -> NotHandled next ctx
    | _ -> NotHandled next ctx

let routes: HttpHandler =
    choose [ GET >=> route "/" >=> Handler
             POST >=> route "/" >=> Handler
             PUT >=> route "/" >=> Handler
             DELETE >=> route "/" >=> Handler ]

let configureAppConfiguration (context: WebHostBuilderContext) (config: IConfigurationBuilder) =
    let environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    let environment' =  if String.IsNullOrEmpty(environment) then "Production" else environment
    config.AddJsonFile("appsettings.json", false, true)
          .AddJsonFile(sprintf "appsettings.%s.json" environment', true)
          .AddEnvironmentVariables()
    |> ignore


let configureApp (app: IApplicationBuilder) =
    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe routes

let configureServices (services: IServiceCollection) =
    // Add Giraffe dependencies
    services.AddGiraffe() |> ignore

