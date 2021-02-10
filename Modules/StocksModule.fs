namespace OttoBot

open Discord.Commands
open Microsoft.Extensions.DependencyInjection
open System
open System.Net.Http
open System.Runtime.InteropServices
open System.Web


module StocksModule =

    type TimePeriod =
        | Day = 0
        | Week = 1
        | Month = 2
        | Year = 3

    type public Module(services: IServiceProvider) =

        inherit ModuleBase<SocketCommandContext>()
        
        /// Use to make HTTP requests.
        let http = services.GetRequiredService<HttpClient>()
        
        /// Use to access 'base.Context', which is normally inaccessible from async computation expressions.
        member private this.Context() =
            base.Context
            
        [<Command("stonk")>]
        [<Summary("Tally your losses.")>]
        member this.StockChart
            (
                [<Summary("The stock ticker. Must be available on BigCharts.")>]
                symbol: string,
                
                [<Optional>]
                [<DefaultParameterValue(TimePeriod.Week)>]
                [<Summary("Specify one of day, week, month, or year. Defaults to week.")>]
                period: TimePeriod
            )
            =
            FSharp.toUnitTask this._StockChart (symbol, period)

        member this._StockChart(symbol, period) =
            async {
                let qs = HttpUtility.ParseQueryString String.Empty
                qs.Add("symb", symbol)
                qs.Add("type", "4")
                qs.Add("style", "330")
                qs.Add
                    (
                        "time",
                        match period with
                        | TimePeriod.Day -> "1"
                        | TimePeriod.Week -> "3"
                        | TimePeriod.Month -> "5"
                        | TimePeriod.Year | _ -> "8"
                    )
                qs.Add
                    (
                        "freq",
                        match period with
                        | TimePeriod.Day -> "7"
                        | TimePeriod.Week -> "8"
                        | TimePeriod.Month -> "1"
                        | TimePeriod.Year | _ -> "2"
                    )

                let! response =
                    http.GetAsync($"https://api.wsj.net/api/kaavio/charts/big.chart?{qs}")
                    |> Async.AwaitTask
                response.EnsureSuccessStatusCode() |> ignore
                
                let ctx = this.Context()
                let stream = response.Content.ReadAsStreamAsync() |> Async.AwaitTask
                let filename = $"{symbol}_{DateTime.UtcNow:yyyyMMdd_HHmm}_{period}.gif"
                do! ctx.Channel.SendFileAsync(stream |> Async.RunSynchronously, filename)
                    |> Async.AwaitTask
                    |> FSharp.ensureSuccess
            }