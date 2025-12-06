module OttoBot.Modules.Public

open Discord
open Discord.Interactions
open FSharp.Data
open FSharp.Data.JsonExtensions
open OttoBot.Helpers
open System
open System.Runtime.InteropServices
open System.Text
open System.Text.RegularExpressions
open System.Threading.Tasks

type private Flight =
    { Origin: string
      Ident: string
      Aircraft: string
      Estimated: string }

type ChartTimePeriod =
    | Day = 0
    | Week = 1
    | Month = 2
    | Year = 3

type Module(handler) =
    inherit FSharpModule(handler)

    member val private Rng = new Random()

    // It's important that the return value of each method be cast to a Task.
    // An F# task computation expression returns a Task<unit>, which Discord.Net
    // rejects as an invalid type signature for a command handler.

    [<SlashCommand("ping", "Run a welfare check")>]
    member this.PingPong() : Task =
        task {
            let ping = this.Context.Client.Latency
            return! this.RespondAsync $"Pong!\nSocket latency: {ping}ms"
        }

    [<SlashCommand("bob", "Write sPoNgEbOb tExT")>]
    member this.SpongeBob([<Summary(description = "The text to transform")>] text: string) : Task =
        task {
            let (|Upper|Lower|NonAlpha|) c =
                let i = int c

                if i >= 65 && i < 91 then Upper
                else if i >= 97 && i < 123 then Lower
                else NonAlpha

            let alternate s =
                let rec _alternate isUpper (accum: StringBuilder) s =
                    match s with
                    | "" -> accum
                    | _ ->
                        let c = s.[0]

                        let first =
                            match c with
                            | Upper -> if isUpper then c else char (int c + 32)
                            | Lower -> if isUpper then char (int c - 32) else c
                            | NonAlpha -> c

                        let upper =
                            match c with
                            | Upper
                            | Lower -> not isUpper
                            | NonAlpha -> isUpper

                        _alternate upper (accum.Append(string first)) s.[1..]

                let sb = _alternate false (StringBuilder()) s
                sb.ToString()

            return! this.RespondAsync(alternate text)
        }

    [<SlashCommand("roll", "Roll an n-sided die")>]
    member this.Roll
        ([<Summary(description = "The number of sides"); Optional; DefaultParameterValue(6u)>] sides: uint)
        : Task =
        task {
            let n = this.Rng.Next(1, int sides)
            return! this.RespondAsync $"This {sides}-sided die rolls a **{n}**!"
        }

    [<SlashCommand("1984", "Literally...")>]
    member this.Orwell() : Task =
        task {
            let text =
                "**Literally...**
⠀⠀⠀⠀⠀⠀⠀⣠⡀⠀⠀⠀⠀⠀⠀⠀⠀⢰⠤⠤⣄⣀⡀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⢀⣾⣟⠳⢦⡀⠀⠀⠀⠀⠀⠀⢸⠀⠀⠀⠀⠉⠉⠉⠉⠉⠒⣲⡄
⠀⠀⠀⠀⠀⣿⣿⣿⡇⡇⡱⠲⢤⣀⠀⠀⠀⢸⠀⠀⠀1984⠀⣠⠴⠊⢹⠁
⠀⠀⠀⠀⠀⠘⢻⠓⠀⠉⣥⣀⣠⠞⠀⠀⠀⢸⠀⠀⠀⠀⢀⡴⠋⠀⠀⠀⢸⠀
⠀⠀⠀⠀⢀⣀⡾⣄⠀⠀⢳⠀⠀⠀⠀⠀⠀⢸⢠⡄⢀⡴⠁⠀⠀⠀⠀⠀⡞⠀
⠀⠀⠀⣠⢎⡉⢦⡀⠀⠀⡸⠀⠀⠀⠀⠀⢀⡼⣣⠧⡼⠀⠀⠀⠀⠀⠀⢠⠇⠀
⠀⢀⡔⠁⠀⠙⠢⢭⣢⡚⢣⠀⠀⠀⠀⠀⢀⣇⠁⢸⠁⠀⠀⠀⠀⠀⠀⢸⠀⠀
⠀⡞⠀⠀⠀⠀⠀⠀⠈⢫⡉⠀⠀⠀⠀⢠⢮⠈⡦⠋⠀⠀⠀⠀⠀⠀⠀⣸⠀⠀
⢀⠇⠀⠀⠀⠀⠀⠀⠀⠀⠙⢦⡀⣀⡴⠃⠀⡷⡇⢀⡴⠋⠉⠉⠙⠓⠒⠃⠀⠀
⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠁⠀⠀⡼⠀⣷⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⡞⠀⠀⠀⠀⠀⠀⠀⣄⠀⠀⠀⠀⠀⠀⡰⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⢧⠀⠀⠀⠀⠀⠀⠀⠈⠣⣀⠀⠀⡰⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀"

            return! this.RespondAsync text
        }

    [<SlashCommand("ops", "Avengers, assemble!")>]
    member this.Ops
        ([<Summary(description = "Your message"); Optional; DefaultParameterValue("")>] message: string)
        : Task =
        task {
            let text =
                "```
██████╗ ██████╗ ███████╗██████╗ ██╗██████╗ ██╗
██╔═══██╗██╔══██╗██╔════╝╚════██╗██║╚════██╗██║
██║   ██║██████╔╝███████╗  ▄███╔╝██║  ▄███╔╝██║
██║   ██║██╔═══╝ ╚════██║  ▀▀══╝ ╚═╝  ▀▀══╝ ╚═╝
╚██████╔╝██║     ███████║  ██╗   ██╗  ██╗   ██╗
╚═════╝ ╚═╝     ╚══════╝  ╚═╝   ╚═╝  ╚═╝   ╚═╝
```"

            return! this.RespondAsync(text + message)
        }

    [<SlashCommand("flights", "Create an arrivals board for a given airport")>]
    member this.Flights([<Summary(description = "The ICAO code of the airport")>] icao: string) : Task =
        task {
            let numFlights = 10

            let parseFlight (row: HtmlNode) =
                let linkText (el: HtmlNode) =
                    match Seq.tryHead (el.CssSelect "a") with
                    | Some link -> link.InnerText()
                    | None -> ""

                let cells = row.CssSelect "td"

                if cells.Length = 6 then
                    Some
                        { Origin = linkText cells.[2]
                          Ident = linkText cells.[0]
                          Aircraft = linkText cells.[1]
                          Estimated = cells.[5].InnerText() }
                else
                    None

            let parseAllFlights (doc: HtmlDocument) =
                match Seq.tryHead (doc.CssSelect ".airportBoard[data-type='arrivals']") with
                | Some table ->
                    let summary =
                        match Seq.tryHead (doc.CssSelect "h1") with
                        | Some head -> head.InnerText()
                        | None -> ""

                    // Header rows are nested in the <thead> element, while data rows are direct descendants.
                    let rows = Seq.filter (fun (el: HtmlNode) -> el.Name() = "tr") (table.Elements())
                    summary, Seq.choose parseFlight rows
                | None -> "No Data", Seq.empty

            do! this.DeferAsync()

            let! doc = HtmlDocument.AsyncLoad $"https://flightaware.com/live/airport/{icao}"
            let summary, flights = parseAllFlights doc

            let makeTable flights =
                seq {
                    yield Data [ "Flight"; "Type"; "From"; "ETA" ]
                    yield Separator

                    yield!
                        Seq.map
                            (fun flight -> Data [ flight.Ident; flight.Aircraft; flight.Origin; flight.Estimated ])
                            flights
                }
                |> makeTable "-" " | "

            let table = makeTable (Seq.truncate numFlights flights)
            return! this.FollowupAsync $"{summary}:\n{table}"
        }

    [<SlashCommand("metar", "Check the weather for a given airport")>]
    member this.Metar([<Summary(description = "The ICAO code of the airport")>] icao: string) : Task =
        task {
            do! this.DeferAsync()

            let! response =
                Http.AsyncRequestString $"https://aviationweather.gov/api/data/metar?ids={icao}&format=decoded"

            let text = Regex.Replace(response, @"^  (\w+)", "  **$1**", RegexOptions.Multiline)

            return! this.FollowupAsync text
        }

    [<SlashCommand("stonk", "Tally your losses")>]
    member this.Stonk
        (
            [<Summary(description = "The stock ticker; must be available on BigCharts")>] symbol: string,
            [<Summary(description = "The time period for the chart");
              Optional;
              DefaultParameterValue(ChartTimePeriod.Week)>] period: ChartTimePeriod
        ) : Task =
        task {
            do! this.DeferAsync()

            let qs =
                [ "symb", symbol
                  "type", "4"
                  "style", "330"
                  "time",
                  match period with
                  | ChartTimePeriod.Day -> "1"
                  | ChartTimePeriod.Week -> "3"
                  | ChartTimePeriod.Month -> "5"
                  | ChartTimePeriod.Year
                  | _ -> "8"
                  "freq",
                  match period with
                  | ChartTimePeriod.Day -> "7"
                  | ChartTimePeriod.Week -> "8"
                  | ChartTimePeriod.Month -> "1"
                  | ChartTimePeriod.Year
                  | _ -> "2" ]

            let! response = Http.AsyncRequestStream("https://api.wsj.net/api/kaavio/charts/big.chart", qs)
            let chartStream = response.ResponseStream

            let! response =
                JsonValue.AsyncLoad
                    $"https://api.wsj.net/api/autocomplete/search?entitlementToken=cecc4267a0194af89ca343805a3e57af&q={symbol}"

            let description =
                match response?symbols with
                | JsonValue.Array results ->
                    match Array.tryHead results with
                    | Some first -> first?company.AsString()
                    | None -> ""
                | _ -> ""

            use attachment =
                new FileAttachment(chartStream, $"{description}_{DateTime.UtcNow:yyyyMMdd_HHmm}_{period}.gif")

            return!
                this.FollowupWithFilesAsync(
                    Seq.singleton attachment,
                    $"**{symbol.ToUpperInvariant()}**: {description}"
                )
        }

    [<SlashCommand("vx", "Use a better embed for a Reddit, X, or TikTok post")>]
    member this.MakeVx([<Summary(description = "The URL to vx-ify")>] url: string) : Task =
        task {
            do! this.DeferAsync()

            // Follow any 302 redirects to the canonical URL to maximize cache hits.
            let! response =
                Http.AsyncRequest(
                    url,
                    [],
                    [ ("User-Agent",
                       "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36 ") ]
                )

            let uri = Uri response.ResponseUrl
            let host = uri.Host.ToLowerInvariant().Split "."

            let newHost =
                match $"{host[host.Length - 2]}.{host[host.Length - 1]}" with
                | "reddit.com" -> "vxreddit.com"
                | "tiktok.com" -> "vxtiktok.com"
                | "x.com"
                | "twitter.com" -> "vxtwitter.com"
                | _ -> uri.Host

            // Query strings are mostly useless. Just drop them.
            let newUri = Uri(Uri $"https://{newHost}", uri.AbsolutePath)
            return! this.FollowupAsync(newUri.ToString())
        }
