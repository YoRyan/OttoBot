using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace OttoBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient Client;
        private readonly InteractionService Commands;
        private readonly IServiceProvider Services;

        public CommandHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
        {
            Client = client;
            Commands = commands;
            Services = services;
        }

        public async Task InitializeAsync()
        {
            // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);

            Client.InteractionCreated += HandleInteraction;

            Commands.SlashCommandExecuted += SlashCommandExecuted;
        }

        private async Task SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                // Reply with more details about the error.
                await arg2.Interaction.RespondAsync(arg3.ErrorReason);
            }
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var ctx = new SocketInteractionContext(Client, arg);
                await Commands.ExecuteCommandAsync(ctx, Services);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
