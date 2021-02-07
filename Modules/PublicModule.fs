﻿namespace OttoBot

open Discord.Commands
open Microsoft.Extensions.DependencyInjection
open System
open System.Net.Http
open System.Runtime.InteropServices
open System.Web


type public PublicModule(services: IServiceProvider) =

    inherit ModuleBase<SocketCommandContext>()

    /// Use to make HTTP requests.
    let http = services.GetRequiredService<HttpClient>()

    /// Use to access 'base.Context', which is normally inaccessible from public methods.
    member private this.Context() =
        base.Context
        
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

    member this._StockChart(symbol: string, period: string) =
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