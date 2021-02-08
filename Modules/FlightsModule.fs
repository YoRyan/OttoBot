namespace OttoBot

open Discord.Commands
open FSharp.Data
open FSharp.Data.JsonExtensions
open System
open System.Web


module FlightsModule =

    type Aircraft =
        { Type: string
          Registration: string }

    type Flight =
        { Departure: string
          Airline: string
          Number: string
          Aircraft: Aircraft option
          Scheduled: DateTime
          Estimated: DateTime }
          
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
                let parse (record: JsonValue) =
                    let arrival = record?arrival
                    let aircraft = record?aircraft
                    {
                        Departure = record?departure?icao.AsString()
                        Airline = record?airline?name.AsString()
                        Number = record?flight?number.AsString()
                        Aircraft =
                            match aircraft with
                            | JsonValue.Record _ ->
                                Some {
                                    Type = aircraft?icao.AsString()
                                    Registration = aircraft?registration.AsString()
                                }
                            | _ -> None
                        Scheduled = arrival?scheduled.AsDateTime()
                        Estimated = arrival?estimated.AsDateTime()
                    }

                let qs = HttpUtility.ParseQueryString String.Empty
                qs.Add("access_key", Environment.GetEnvironmentVariable("AVSTACK_KEY"))
                qs.Add("flight_status", "active")
                qs.Add("arr_icao", icao)
                let url = $"http://api.aviationstack.com/v1/flights?{qs}"

                let allFlights =
                    Seq.sortBy
                        (fun flight -> flight.Estimated)
                        (Seq.map parse ((JsonValue.Load url)?data.AsArray()))
                let latestFlights = Seq.truncate numFlights allFlights
                
                let table =
                    seq {
                        yield Discord.TableRow.Data(fields = [ "From"; "Flight"; "ETA"; "Type" ])
                        yield Discord.TableRow.Separator

                        for flight in latestFlights do
                            let aircraft =
                                match flight.Aircraft with
                                | Some(ac) -> ac.Type
                                | None -> ""
                            yield Discord.TableRow.Data
                                (
                                    [ flight.Departure;
                                      $"{flight.Airline} {flight.Number}";
                                      flight.Estimated.ToString("M/d HH:mm");
                                      aircraft ]
                                )
                    }
                    |> Discord.makeTable "-" " | "

                let ctx = this.Context()
                do! ctx.Channel.SendMessageAsync($"Incoming flights at {icao.ToUpper()}:\n{table}")
                    |> Async.AwaitTask
                    |> FSharp.ensureSuccess
            }