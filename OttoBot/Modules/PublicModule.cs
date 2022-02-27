using Discord.Interactions;

namespace OttoBot.Modules
{
    public class PublicModule : FSharpInteropModule
    {
        public HttpClient? HttpClient { get; set; }

        [SlashCommand("ping", "Run a welfare check")]
        public async Task PingPong()
        {
            await FSharpInteractAsync(OttoCompute.PublicModule.pingPong(
                ping: Context.Client.Latency
            ));
        }

        [SlashCommand("bob", "Write sPoNgEbOb tExT")]
        public async Task SpongeBob([Summary(description: "The text to transform")] string text)
        {
            await FSharpInteractAsync(OttoCompute.PublicModule.spongeBob(
                text: text
            ));
        }

        [SlashCommand("roll", "Roll an n-sided die")]
        public async Task Roll([Summary(description: "The number of sides")] uint sides = 6)
        {
            await FSharpInteractAsync(OttoCompute.PublicModule.roll(
                sides: (int)sides
            ));
        }

        [SlashCommand("1984", "Literally...")]
        public async Task Orwell()
        {
            await FSharpInteractAsync(OttoCompute.PublicModule.orwell());
        }

        [SlashCommand("ops", "Avengers, assemble!")]
        public async Task Ops()
        {
            await FSharpInteractAsync(OttoCompute.PublicModule.ops());
        }

        [SlashCommand("flights", "Create an arrivals board for a given airport")]
        public async Task Flights([Summary(description: "The ICAO code of the airport")] string icao)
        {
            await FSharpInteractAsync(OttoCompute.PublicModule.flights(
                icao: icao
            ));
        }

        [SlashCommand("stonk", "Tally your losses")]
        public async Task Stonk([Summary(description: "The stock ticker; must be available on BigCharts")]
                                string symbol,
                                [Summary(description: "The time period for the chart")]
                                OttoCompute.PublicModule.ChartTimePeriod period = OttoCompute.PublicModule.ChartTimePeriod.Week)
        {
            await FSharpInteractAsync(OttoCompute.PublicModule.stonk(
                http: HttpClient,
                symbol: symbol,
                period: period
            ));
        }
    }
}
