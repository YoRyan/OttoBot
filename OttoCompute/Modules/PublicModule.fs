namespace OttoCompute

open FSharp.Control
open FSharp.Data
open System
open System.Net.Http
open System.Text
open System.Web

module PublicModule =

    let pingPong ping =
        asyncSeq { yield Response(text = $"Pong!\nSocket latency: {ping} ms") }

    let spongeBob text =
        asyncSeq {
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

            yield Response(text = alternate text)
        }

    let roll sides =
        asyncSeq {
            let n = (new Random()).Next(1, sides)
            yield Response(text = $"This {sides}-sided die rolls a **{n}**!")
        }

    let orwell () =
        asyncSeq {
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

            yield Response(text = text)
        }

    let ops message =
        asyncSeq {
            let text =
                "```
 ██████╗ ██████╗ ███████╗██████╗ ██╗██████╗ ██╗
██╔═══██╗██╔══██╗██╔════╝╚════██╗██║╚════██╗██║
██║   ██║██████╔╝███████╗  ▄███╔╝██║  ▄███╔╝██║
██║   ██║██╔═══╝ ╚════██║  ▀▀══╝ ╚═╝  ▀▀══╝ ╚═╝
╚██████╔╝██║     ███████║  ██╗   ██╗  ██╗   ██╗
 ╚═════╝ ╚═╝     ╚══════╝  ╚═╝   ╚═╝  ╚═╝   ╚═╝
```"

            yield Response(text = text + message)
        }

    type Flight =
        { Origin: string
          Ident: string
          Aircraft: string
          Estimated: string }

    let flights icao =
        asyncSeq {
            let numFlights = 10

            let parseFlight (row: HtmlNode) =
                let linkText (el: HtmlNode) =
                    match Seq.tryHead (el.CssSelect("a")) with
                    | Some link -> link.InnerText()
                    | None -> ""

                let cells = row.CssSelect("td")

                if cells.Length = 6 then
                    Some
                        { Origin = linkText cells.[2]
                          Ident = linkText cells.[0]
                          Aircraft = linkText cells.[1]
                          Estimated = cells.[5].InnerText() }
                else
                    None

            let parseAllFlights (doc: HtmlDocument) =
                match Seq.tryHead (doc.CssSelect(".prettyTable")) with
                | Some table ->
                    let summary =
                        match Seq.tryHead (table.CssSelect("h1")) with
                        | Some head -> head.InnerText()
                        | None -> ""

                    let rows = Seq.filter (fun (el: HtmlNode) -> el.Name() = "tr") (table.Elements())
                    (summary, Seq.choose parseFlight rows)
                | None -> ("No Data", Seq.empty)

            let! doc = HtmlDocument.AsyncLoad $"https://flightaware.com/live/airport/{icao}/enroute"
            let summary, flights = parseAllFlights doc

            let makeTable flights =
                seq {
                    yield Helpers.TableRow.Data([ "Flight"; "Type"; "From"; "ETA" ])
                    yield Helpers.TableRow.Separator

                    yield!
                        Seq.map
                            (fun flight ->
                                Helpers.TableRow.Data(
                                    [ flight.Ident
                                      flight.Aircraft
                                      flight.Origin
                                      flight.Estimated ]
                                ))
                            flights
                }
                |> Helpers.makeTable "-" " | "

            let table = makeTable (Seq.truncate numFlights flights)

            yield Response(text = $"{summary}:\n{table}")
        }

    type ChartTimePeriod =
        | Day = 0
        | Week = 1
        | Month = 2
        | Year = 3

    let stonk (http: HttpClient) symbol period =
        asyncSeq {
            let qs = HttpUtility.ParseQueryString String.Empty
            qs.Add("symb", symbol)
            qs.Add("type", "4")
            qs.Add("style", "330")

            qs.Add(
                "time",
                match period with
                | ChartTimePeriod.Day -> "1"
                | ChartTimePeriod.Week -> "3"
                | ChartTimePeriod.Month -> "5"
                | ChartTimePeriod.Year
                | _ -> "8"
            )

            qs.Add(
                "freq",
                match period with
                | ChartTimePeriod.Day -> "7"
                | ChartTimePeriod.Week -> "8"
                | ChartTimePeriod.Month -> "1"
                | ChartTimePeriod.Year
                | _ -> "2"
            )

            let! response =
                http.GetAsync($"https://api.wsj.net/api/kaavio/charts/big.chart?{qs}")
                |> Async.AwaitTask

            response.EnsureSuccessStatusCode() |> ignore

            let filename = $"{symbol}_{DateTime.UtcNow:yyyyMMdd_HHmm}_{period}.gif"

            let! stream =
                response.Content.ReadAsStreamAsync()
                |> Async.AwaitTask

            yield ResponseWithFile(filename = filename, stream = stream)
        }
