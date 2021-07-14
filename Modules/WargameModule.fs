namespace OttoBot

open Discord.Commands
open System.Runtime.InteropServices

type public WargameModule(commands: CommandService) =

    inherit ModuleBase<SocketCommandContext>()

    /// Use to access 'base.Context', which is normally inaccessible from async computation expressions.
    member private this.Context() =
        base.Context

    [<Command("ops")>]
    [<Summary("Avengers, assemble!")>]
    member this.Ops
        (
            [<Optional>]
            [<Remainder>]
            [<Summary("The text to say afterwards.")>]
            text: string
        )
        =
        FSharp.toUnitTask this._Ops text

    member private this._Ops(text: string) =
        async {
            let ctx = this.Context()
            let ops = "```
 ██████╗ ██████╗ ███████╗██████╗ ██╗██████╗ ██╗
██╔═══██╗██╔══██╗██╔════╝╚════██╗██║╚════██╗██║
██║   ██║██████╔╝███████╗  ▄███╔╝██║  ▄███╔╝██║
██║   ██║██╔═══╝ ╚════██║  ▀▀══╝ ╚═╝  ▀▀══╝ ╚═╝
╚██████╔╝██║     ███████║  ██╗   ██╗  ██╗   ██╗
 ╚═════╝ ╚═╝     ╚══════╝  ╚═╝   ╚═╝  ╚═╝   ╚═╝
```"
            do! ctx.Channel.SendMessageAsync(ops + text)
                |> Async.AwaitTask
                |> FSharp.ensureSuccess
        }

