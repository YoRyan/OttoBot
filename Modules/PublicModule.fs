namespace OttoBot

open Discord.Commands
open Microsoft.Extensions.DependencyInjection
open System
open System.Net.Http
open System.Runtime.InteropServices
open System.Web


type public PublicModule(services: IServiceProvider, commands: CommandService) =

    inherit ModuleBase<SocketCommandContext>()

    /// Use to make HTTP requests.
    let http = services.GetRequiredService<HttpClient>()

    /// Use to access 'base.Context', which is normally inaccessible from async computation expressions.
    member private this.Context() =
        base.Context
        
    [<Command("help")>]
    [<Summary("Get help with this bot, or with a command if specified.")>]
    member this.Help
        (
            [<Optional>]
            [<DefaultParameterValue("")>]
            [<Summary("Get help for a specific command.")>]
            command: string
        )
        =
        FSharp.toUnitTask this._Help command

    member private this._Help(command) =
        async {
            let ctx = this.Context()

            let singleHelp (cmd: CommandInfo) =
                async {
                    let oneParam (param: ParameterInfo) =
                        if param.IsOptional then $"[{param.Name}]"
                        else $"<{param.Name}>"
                    let allParams = Seq.map oneParam cmd.Parameters |> String.concat " "

                    let nameLength =
                        Seq.max
                            (
                                Seq.map
                                    (fun (param: ParameterInfo) -> param.Name.Length)
                                    cmd.Parameters
                            )

                    let lines =
                        Seq.map
                            (fun (param: ParameterInfo) ->
                                $"{param.Name.PadRight(nameLength, ' ')} : {param.Summary}")
                            cmd.Parameters
                        |> String.concat "\n"

                    do! ctx.Channel.SendMessageAsync($"arguments: `{allParams}`\n```{lines}```")
                        |> Async.AwaitTask
                        |> FSharp.ensureSuccess
                }

            let allHelp =
                async {
                    let nameLength =
                        Seq.max
                            (
                                Seq.map
                                    (fun (cmd: CommandInfo) -> cmd.Name.Length)
                                    commands.Commands
                            )

                    let lines =
                        Seq.map
                            (fun (cmd: CommandInfo) ->
                                $"{cmd.Name.PadRight(nameLength, ' ')} : {cmd.Summary}")
                            commands.Commands
                        |> String.concat "\n"

                    do! ctx.Channel.SendMessageAsync($"Here are my commands:\n```{lines}```")
                        |> Async.AwaitTask
                        |> FSharp.ensureSuccess
                }

            let aliases (cmd: CommandInfo) =
                Seq.map (fun a -> (a, cmd)) cmd.Aliases
            let map = Map (Seq.collect aliases commands.Commands)

            do! match map.TryFind command with
                | Some cmd -> singleHelp cmd
                | None -> allHelp
        }

    [<Command("ping")>]
    [<Summary("Run a welfare check.")>]
    member this.PingPong() =
        FSharp.toUnitTask this._PingPong ()

    member private this._PingPong() =
        async {
            let ctx = this.Context()
            do! ctx.Channel.SendMessageAsync("Pong!")
                |> Async.AwaitTask
                |> FSharp.ensureSuccess
        }
        
    [<Command("bob")>]
    [<Summary("Write sPoNgEbOb tExT!")>]
    member this.SpongeBob
        (
            [<Remainder>]
            [<Summary("The text to transform.")>]
            text: string
        )
        =
        FSharp.toUnitTask this._SpongeBob text

    member private this._SpongeBob(text: string) =
        async {
            let isUpper c =
                let i = int c
                i >= 65 && i < 91

            let isLower c =
                let i = int c
                i >= 97 && i < 123

            let upper c =
                if isLower c then char (int c - 32)
                else c

            let lower c =
                if isUpper c then char (int c + 32)
                else c

            let mutable altUpper = true
            let alternate c =
                altUpper <- not altUpper
                if altUpper then upper c
                else lower c

            let alternateLetters c =
                if isUpper c || isLower c then alternate c
                else c
                
            let ctx = this.Context()
            do! ctx.Channel.SendMessageAsync(String.map alternateLetters text)
                |> Async.AwaitTask
                |> FSharp.ensureSuccess
        }
        
    [<Command("stonk")>]
    [<Summary("Tally your losses.")>]
    member this.StockChart
        (
            [<Summary("The stock ticker. Must be available on BigCharts.")>]
            symbol: string,
        
            [<Optional>]
            [<DefaultParameterValue("")>]
            [<Summary("Specify one of day, week, month, or year. Defaults to week.")>]
            period: string
        )
        =
        FSharp.toUnitTask this._StockChart (symbol, period)

    member this._StockChart(symbol, period) =
        async {
            let invPeriod = String.map Char.ToLowerInvariant period
            let time =
                match invPeriod with
                | "day" -> "1"
                | "month" -> "5"
                | "year" -> "8"
                | "week" | _ -> "3"
            let freq =
                match invPeriod with
                | "day" -> "7"
                | "month" -> "1"
                | "year" -> "2"
                | "week" | _ -> "8"

            let qs = HttpUtility.ParseQueryString String.Empty
            qs.Add("symb", symbol)
            qs.Add("type", "4")
            qs.Add("style", "330")
            qs.Add("time", time)
            qs.Add("freq", freq)

            let! response =
                http.GetAsync($"https://api.wsj.net/api/kaavio/charts/big.chart?{qs}")
                |> Async.AwaitTask
            response.EnsureSuccessStatusCode() |> ignore
            
            let ctx = this.Context()
            let stream = response.Content.ReadAsStreamAsync() |> Async.AwaitTask
            let filename = $"{symbol}_{DateTime.UtcNow:yyyyMMdd_HHmm}_{time}.gif"
            do! ctx.Channel.SendFileAsync(stream |> Async.RunSynchronously, filename)
                |> Async.AwaitTask
                |> FSharp.ensureSuccess
        }