namespace OttoBot

open Discord.Commands
open System
open System.Runtime.InteropServices
open System.Text


type public PublicModule(commands: CommandService) =

    inherit ModuleBase<SocketCommandContext>()

    let rng = new Random()

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

                    let table =
                        seq {
                            for param in cmd.Parameters do
                                yield Discord.TableRow.Data([ param.Name; param.Summary ])
                        }
                        |> Discord.makeTable "-" " : "

                    do! ctx.Channel.SendMessageAsync($"arguments: `{allParams}`\n{table}")
                        |> Async.AwaitTask
                        |> FSharp.ensureSuccess
                }

            let allHelp =
                async {
                    let table =
                        seq {
                            for cmd in commands.Commands do
                                yield Discord.TableRow.Data([ cmd.Name; cmd.Summary ])
                        }
                        |> Discord.makeTable "-" " : "

                    do! ctx.Channel.SendMessageAsync($"Here are my commands:\n{table}")
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
            do! ctx.Channel.SendMessageAsync($"Pong!\nSocket latency: {ctx.Client.Latency} ms")
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
                            | Upper | Lower -> not isUpper
                            | NonAlpha -> isUpper
                        _alternate upper (accum.Append(string first)) s.[1..]
                let sb = _alternate false (StringBuilder()) s
                sb.ToString()

            let ctx = this.Context()
            do! ctx.Channel.SendMessageAsync(alternate text)
                |> Async.AwaitTask
                |> FSharp.ensureSuccess
        }

    [<Command("1984")>]
    [<Summary("Literally...")>]
    member this.Orwell() =
        FSharp.toUnitTask this._Orwell ()

    member private this._Orwell() =
        async {
            let ctx = this.Context()
            let text = "**Literally...**
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
            do! ctx.Channel.SendMessageAsync(text)
                |> Async.AwaitTask
                |> FSharp.ensureSuccess
        }

    [<Command("roll")>]
    [<Summary("Roll an n-sided die.")>]
    member this.Roll
        (
            [<Optional>]
            [<DefaultParameterValue(6)>]
            [<Summary("Specify the number of sides.")>]
            sides: int
        )
        =
        FSharp.toUnitTask this._Roll sides

    member private this._Roll(sides: int) =
        async {
            let res = rng.Next(1, sides)
            let ctx = this.Context()
            do! ctx.Channel.SendMessageAsync($"This {sides}-sided die rolls a **{res}**!")
                |> Async.AwaitTask
                |> FSharp.ensureSuccess
        }