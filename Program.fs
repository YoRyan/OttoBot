namespace OttoBot

open Discord
open Discord.Commands
open Discord.WebSocket
open Microsoft.Extensions.DependencyInjection
open Microsoft.FSharp.Core
open System
open System.Net.Http


module Bot =

    let configureServices () =
        (new ServiceCollection())
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<Otto>()
            .AddSingleton<HttpClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<PublicModule>()
            .BuildServiceProvider()
            
    let log msg =
        async {
            printfn "%s" (msg.ToString())
        }
        
    let waitIndefinitely () =
        async {
            let evt = Event<_>().Publish
            do! Async.AwaitEvent(evt) |> Async.Ignore
        }

    [<EntryPoint>]
    let main argv =
        async {
            use services = configureServices ()

            let client = services.GetRequiredService<DiscordSocketClient>()
            client.add_Log
                (
                    FSharp.FuncHelpers.ToUnitDelegate<LogMessage> log
                )

            let token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")
            do! client.LoginAsync(TokenType.Bot, token) |> Async.AwaitTask
            do! client.StartAsync() |> Async.AwaitTask

            let bot = services.GetRequiredService<Otto>()
            do! bot.InitializeAsync()

            do! waitIndefinitely()
        }
        |> Async.RunSynchronously
        0
