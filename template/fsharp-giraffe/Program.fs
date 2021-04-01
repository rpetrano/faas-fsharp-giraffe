module FunctionServer

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open FunctionHttp

[<EntryPoint>]
let main _ =
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun webHostBuilder ->
        webHostBuilder.ConfigureAppConfiguration(configureAppConfiguration).Configure(configureApp)
                      .ConfigureServices(configureServices)
        |> ignore).Build().Run()
    0
