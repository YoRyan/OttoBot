namespace OttoBot

open Discord
open Discord.Commands
open Discord.WebSocket
open Microsoft.Extensions.DependencyInjection
open Microsoft.FSharp.Core
open System
open System.Reflection

type Otto(services: IServiceProvider) =

    let commands = services.GetRequiredService<CommandService>()
    let client = services.GetRequiredService<DiscordSocketClient>()

    let handleCommandAsync (msg: SocketMessage) =
        let prefix = '.'

        let (|Command|_|) (um: SocketUserMessage) =
            let mutable pos = 0
            let mention =
                (
                    um.HasCharPrefix(prefix, &pos)
                    || um.HasMentionPrefix(client.CurrentUser, &pos)
                )
            if mention && not um.Author.IsBot then Some Command else None

        async {
            match msg with
            | :? SocketUserMessage as uMsg ->
                match uMsg with
                | Command ->
                    let ctx = new SocketCommandContext(client, uMsg)

                    let mutable pos = 0
                    uMsg.HasCharPrefix(prefix, &pos) |> ignore

                    do! commands.ExecuteAsync(ctx, pos, services)
                        |> Async.AwaitTask
                        |> FSharp.ensureSuccess
                    ()
                | _ -> ()
            | _ -> ()
        }

    let commandExecutedAsync (command: Optional<CommandInfo>, ctx: ICommandContext, result: IResult) =
        async {
            if command.IsSpecified && not result.IsSuccess then
                ctx.Channel.SendMessageAsync($"error: {result}")
                |> Async.AwaitTask
                |> ignore
        }

    member this.InitializeAsync() =
        async {
            client.add_MessageReceived
                (
                    FSharp.FuncHelpers.ToUnitDelegate<SocketMessage> handleCommandAsync
                )
            commands.add_CommandExecuted
                (
                    FSharp.FuncHelpers.ToUnitDelegate<Optional<CommandInfo>, ICommandContext, IResult> commandExecutedAsync
                )

            do! commands.AddModulesAsync(Assembly.GetEntryAssembly(), services)
                |> Async.AwaitTask
                |> FSharp.ensureSuccess
        }