using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OttoCSharp;

public class Program
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _services;

    private readonly DiscordSocketConfig _socketConfig = new()
    {
        // GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
        AlwaysDownloadUsers = true,
    };

    private static readonly InteractionServiceConfig _interactionServiceConfig = new()
    {
        // LocalizationManager = new ResxLocalizationManager("InteractionFramework.Resources.CommandLocales", Assembly.GetEntryAssembly(),
        //     new CultureInfo("en-US"))
    };

    /* Don't use Main because we want the entry point to be in F#.
     * Thus, we have to split the code between the constructor (which can
     * initialize fields) and RunAsync (which is asynchronous). */
    // public static async Task Main(string[] args)
    public Program()
    {
        _configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables(prefix: "DC_")
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        _services = new ServiceCollection()
            .AddSingleton(_configuration)
            .AddSingleton(_socketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>(), _interactionServiceConfig))
            .AddSingleton<InteractionHandler>()
            .BuildServiceProvider();
    }

    public async Task RunAsync()
    {
        var client = _services.GetRequiredService<DiscordSocketClient>();

        client.Log += LogAsync;

        // Here we can initialize the service that will register and execute our commands
        await _services.GetRequiredService<InteractionHandler>()
            .InitializeAsync();

        // Bot token can be provided from the Configuration object we set up earlier
        await client.LoginAsync(TokenType.Bot, _configuration["token"]);
        await client.StartAsync();

        // Never quit the program until manually forced to.
        await Task.Delay(Timeout.Infinite);
    }

    private Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }
}
