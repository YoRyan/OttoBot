namespace OttoBot

open FSharp.Data
open FSharp.Data.JsonExtensions
open NetCord.Rest
open NetCord.Services.ApplicationCommands
open OttoBot.Helpers
open System
open System.Runtime.InteropServices
open System.Text
open System.Text.RegularExpressions
open System.Threading.Tasks

type ChartTimePeriod =
    | Day = 0
    | Week = 1
    | Month = 2
    | Year = 3

type AvcodesLookup =
    | Name = 0
    | ICAO = 1
    | IATA = 2

type Module() =
    inherit ApplicationCommandModule<ApplicationCommandContext>()

    let rng = new Random()

    [<SlashCommand("ping", "Run a welfare check!")>]
    member this.PingPong() : Task =
        task {
            let ping = this.Context.Client.Latency.Milliseconds

            return!
                $"Pong!\nSocket latency: {ping}ms"
                |> InteractionCallback.Message
                |> this.RespondAsync
        }

    [<SlashCommand("bob", "Write sPoNgEbOb tExT")>]
    member this.SpongeBob([<SlashCommandParameter(Description = "The text to transform")>] text: string) : Task =
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

            return! text |> alternate |> InteractionCallback.Message |> this.RespondAsync
        }

    [<SlashCommand("roll", "Roll an n-sided die")>]
    member this.Roll
        ([<SlashCommandParameter(Description = "The number of sides"); Optional; DefaultParameterValue(6u)>] sides: uint)
        : Task =
        task {
            let n = rng.Next(1, int sides)

            return!
                $"This {sides}-sided die rolls a **{n}**!"
                |> InteractionCallback.Message
                |> this.RespondAsync
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

            return! text |> InteractionCallback.Message |> this.RespondAsync
        }

    [<SlashCommand("ops", "Avengers, assemble!")>]
    member this.Ops
        ([<SlashCommandParameter(Description = "Your message"); Optional; DefaultParameterValue("")>] message: string)
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

            return! text + message |> InteractionCallback.Message |> this.RespondAsync
        }

    [<SlashCommand("flights", "Create an arrivals board for a given airport")>]
    member this.Flights([<SlashCommandParameter(Description = "The ICAO code of the airport")>] icao: string) : Task =
        task {
            let parseFlight (row: HtmlNode) =
                let linkText (el: HtmlNode) =
                    match Seq.tryHead (el.CssSelect "a") with
                    | Some link -> link.InnerText()
                    | None -> ""

                let cells = row.CssSelect "td"

                if cells.Length = 6 then
                    Some(
                        Data
                            [ linkText cells.[2]
                              linkText cells.[0]
                              linkText cells.[1]
                              cells.[5].InnerText() ]
                    )
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

            let! _ = InteractionCallback.DeferredMessage() |> this.RespondAsync

            let! doc = HtmlDocument.AsyncLoad $"https://flightaware.com/live/airport/{icao}"
            let summary, flights = parseAllFlights doc

            let table =
                seq {
                    yield Data [ "Flight"; "Type"; "From"; "ETA" ]
                    yield Separator
                    yield! Seq.truncate 10 flights
                }
                |> makeTable "-" " | "

            return! $"{summary}:\n{table}" |> this.FollowupAsync
        }

    [<SlashCommand("metar", "Check the weather for a given airport")>]
    member this.Metar([<SlashCommandParameter(Description = "The ICAO code of the airport")>] icao: string) : Task =
        task {
            let! _ = InteractionCallback.DeferredMessage() |> this.RespondAsync

            let! response =
                Http.AsyncRequestString $"https://aviationweather.gov/api/data/metar?ids={icao}&format=decoded"

            let text = Regex.Replace(response, @"^  (\w+)", "  **$1**", RegexOptions.Multiline)

            return! text |> this.FollowupAsync
        }

    [<SlashCommand("airport", "Look up an airport by name or code")>]
    member this.Airport(query: string, by: AvcodesLookup) : Task =
        task {
            let formField =
                match by with
                | AvcodesLookup.ICAO -> "icaoapt"
                | AvcodesLookup.IATA -> "iataapt"
                | AvcodesLookup.Name
                | _ -> "aptname"

            let! _ = InteractionCallback.DeferredMessage() |> this.RespondAsync

            let! response =
                Http.AsyncRequestStream(
                    "https://www.avcodes.co.uk/aptcoderes.asp",
                    body = FormValues(Seq.singleton (formField, query))

                )

            let doc = HtmlDocument.Load response.ResponseStream

            let table =
                seq {
                    yield Data [ "Airport"; "ICAO"; "IATA"; "Country"; "Province" ]
                    yield Separator

                    yield!
                        doc.CssSelect "table"
                        |> Seq.truncate 10
                        |> Seq.map (fun t ->
                            let cells = t.CssSelect "td"

                            [ 1; 3; 2; 7; 6 ]
                            |> Seq.map (fun i ->
                                match cells.[i].InnerText().Split(":", 2) with
                                | [| _label; data |] -> data
                                | [| data |] -> data
                                | _ -> "")
                            |> Seq.map (fun s -> s.Trim())
                            |> Data)
                }
                |> makeTable "-" " | "

            return! $"Searched for \"**{query}**\":\n{table}" |> this.FollowupAsync
        }

    [<SlashCommand("airline", "Look up an airline by name or code")>]
    member this.Airline(query: string, by: AvcodesLookup) : Task =
        task {
            let formField =
                match by with
                | AvcodesLookup.ICAO -> "icaoairl"
                | AvcodesLookup.IATA -> "iataairl"
                | AvcodesLookup.Name
                | _ -> "airlname"

            let! _ = InteractionCallback.DeferredMessage() |> this.RespondAsync

            let! response =
                Http.AsyncRequestStream(
                    "https://www.avcodes.co.uk/airlcoderes.asp",
                    body = FormValues(Seq.singleton (formField, query))

                )

            let doc = HtmlDocument.Load response.ResponseStream

            let table =
                seq {
                    yield Data [ "Airline"; "Callsign"; "ICAO"; "IATA"; "Country" ]
                    yield Separator

                    yield!
                        doc.CssSelect "table"
                        |> Seq.truncate 10
                        |> Seq.map (fun t ->
                            let cells = t.CssSelect "td"

                            [ 0; 5; 4; 3; 8 ]
                            |> Seq.map (fun i ->
                                match cells.[i].InnerText().Split(":", 2) with
                                | [| _label; data |] -> data
                                | [| data |] -> data
                                | _ -> "")
                            |> Seq.map (fun s -> s.Trim())
                            |> Data)
                }
                |> makeTable "-" " | "

            return! $"Searched for \"**{query}**\":\n{table}" |> this.FollowupAsync
        }

    [<SlashCommand("stonk", "Tally your losses")>]
    member this.Stonk
        (
            [<SlashCommandParameter(Description = "The stock ticker; must be available on BigCharts")>] symbol: string,
            [<SlashCommandParameter(Description = "The time period for the chart");
              Optional;
              DefaultParameterValue(ChartTimePeriod.Week)>] period: ChartTimePeriod
        ) : Task =
        task {
            let! _ = InteractionCallback.DeferredMessage() |> this.RespondAsync

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

            let attachment =
                AttachmentProperties($"{description}_{DateTime.UtcNow:yyyyMMdd_HHmm}_{period}.gif", chartStream)

            return!
                InteractionMessageProperties()
                    .WithContent($"**{symbol.ToUpperInvariant()}**: {description}")
                    .WithAttachments(Seq.singleton attachment)
                |> this.FollowupAsync
        }

    [<SlashCommand("vx", "Use a better embed for a Reddit, X, or TikTok post")>]
    member this.MakeVx([<SlashCommandParameter(Description = "The URL to vx-ify")>] url: string) : Task =
        task {
            let! _ = InteractionCallback.DeferredMessage() |> this.RespondAsync

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
            return! newUri.ToString() |> this.FollowupAsync
        }
