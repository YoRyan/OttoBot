open NetCord
open NetCord.Gateway
open NetCord.Logging
open NetCord.Rest
open NetCord.Services
open NetCord.Services.ApplicationCommands
open System
open System.Threading.Tasks

[<EntryPoint>]
let main args =
    task {
        use client =
            new GatewayClient(
                BotToken(Environment.GetEnvironmentVariable "TOKEN"),
                GatewayClientConfiguration(Logger = ConsoleLogger())
            )

        // Create the application command service
        let applicationCommandService =
            new ApplicationCommandService<ApplicationCommandContext>()

        // Add commands from modules
        applicationCommandService.AddModule typeof<OttoBot.Module>

        let handler (interaction: Interaction) =
            task {
                // Check if the interaction is an application command interaction
                match interaction with
                | :? ApplicationCommandInteraction as applicationCommandInteraction ->
                    // Execute the command
                    let! result =
                        applicationCommandService.ExecuteAsync(
                            ApplicationCommandContext(applicationCommandInteraction, client)
                        )

                    // Check if the execution failed
                    match result with
                    | :? IFailResult as failResult ->
                        // Return the error message to the user if the execution failed
                        let message = $"**Error:** {failResult.Message}"

                        try
                            let! _ = interaction.SendResponseAsync(InteractionCallback.Message message)
                            ()
                        with _ ->
                            ()
                    | _ -> ()
                | _ -> ()
            }
        // Add the handler to handle interactions
        client.add_InteractionCreate (handler >> ValueTask)

        let guildId =
            match Environment.GetEnvironmentVariable "GUILD_ID" with
            | null -> Nullable()
            | s -> s |> Int128.Parse |> uint64 |> Nullable
        // Register the commands so that you can use them in the Discord client
        let! _ = applicationCommandService.RegisterCommandsAsync(client.Rest, client.Id, guildId = guildId)

        do! client.StartAsync()
        do! Task.Delay -1
    }
    |> Async.AwaitTask
    |> Async.RunSynchronously

    0
