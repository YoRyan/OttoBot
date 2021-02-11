namespace OttoBot

open Discord.Commands
open FSharp.Data


module FlightsModule =

    type Flight =
        { Origin: string
          Ident: string
          Aircraft: string
          Estimated: string }

    let numFlights = 10
        
    type public Module() =
    
        inherit ModuleBase<SocketCommandContext>()

        /// Use to access 'base.Context', which is normally inaccessible from async computation expressions.
        member private this.Context() =
            base.Context

        [<Command("flights")>]
        [<Summary("Create an arrivals board for a given airport.")>]
        member this.AirportArrivals
            (
                [<Summary("The ICAO code of the airport.")>]
                icao: string
            )
            =
            FSharp.toUnitTask this._AirportArrivals icao

        member private this._AirportArrivals(icao) =
            async {
                let parseFlight (row: HtmlNode) =
                    let linkText (el: HtmlNode) =
                        match el.CssSelect("a") with
                        | []        -> ""
                        | link :: _ -> link.InnerText()

                    let cells = row.CssSelect("td")
                    match cells.[4].InnerText() with
                    | "" -> None
                    | _  -> Some { Origin = linkText cells.[2]
                                   Ident = linkText cells.[0]
                                   Aircraft = linkText cells.[1]
                                   Estimated = cells.[5].InnerText() }
                    
                let parseAllFlights (doc: HtmlDocument) =
                    let noData = "No Data"
                    match doc.CssSelect(".prettyTable > tr") with
                    | [] -> (noData, Seq.empty)
                    | rows ->
                        let summary =
                            match doc.CssSelect(".prettyTable h1") with
                            | [] -> noData
                            | head :: _ -> head.InnerText()
                        let flights = Seq.choose parseFlight rows
                        (summary, flights)

                let! doc = HtmlDocument.AsyncLoad $"https://flightaware.com/live/airport/{icao}/enroute"
                let summary, flights = parseAllFlights doc
                
                let makeTable flights =
                    seq {
                        yield Discord.TableRow.Data(fields = [ "Flight"; "Type"; "From"; "ETA" ])
                        yield Discord.TableRow.Separator
                        yield!
                            Seq.map
                                (fun flight ->
                                    Discord.TableRow.Data
                                        (
                                            [ flight.Ident;
                                              flight.Aircraft;
                                              flight.Origin;
                                              flight.Estimated ]
                                        )
                                )
                                flights
                    }
                    |> Discord.makeTable "-" " | "
                let table = makeTable (Seq.truncate numFlights flights)

                let ctx = this.Context()
                do! ctx.Channel.SendMessageAsync($"{summary}:\n{table}")
                    |> Async.AwaitTask
                    |> FSharp.ensureSuccess
            }