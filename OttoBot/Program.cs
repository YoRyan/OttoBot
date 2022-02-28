using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OttoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "DC_")
                .Build();

            RunAsync(config).GetAwaiter().GetResult();
        }

        static async Task RunAsync(IConfiguration configuration)
        {
            using var services = ConfigureServices(configuration);

            var client = services.GetRequiredService<DiscordSocketClient>();
            var commands = services.GetRequiredService<InteractionService>();

            client.Log += LogAsync;
            commands.Log += LogAsync;

            client.Ready += async () =>
            {
                if (IsDebug() && ulong.TryParse(configuration["testguild"], out var guildId))
                    await commands.RegisterCommandsToGuildAsync(guildId);
                else
                    await commands.RegisterCommandsGloballyAsync();
            };

            // Setting this flag allows one to clear the bot's globally
            // registered commands (in Release mode) or guild-registered
            // commands (in Debug mode with "testguild" also set).
            if (string.IsNullOrEmpty(configuration["clearcommands"]))
                await services.GetRequiredService<CommandHandler>().InitializeAsync();

            await client.LoginAsync(TokenType.Bot, configuration["token"]);
            await client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        static ServiceProvider ConfigureServices(IConfiguration configuration)
            => new ServiceCollection()
                .AddSingleton(configuration)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(services => new InteractionService(services.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<CommandHandler>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();

        static Task LogAsync(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}
