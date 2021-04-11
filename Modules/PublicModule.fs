namespace OttoBot

open Discord.Commands
open System
open System.Runtime.InteropServices


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