namespace OttoBot

open Discord.Commands
open System
open System.Text

type public WargameModule(commands: CommandService) =

    inherit ModuleBase<SocketCommandContext>()

    /// Use to access 'base.Context', which is normally inaccessible from async computation expressions.
    member private this.Context() =
        base.Context

    [<Command("ops")>]
    [<Summary("Avengers, assemble!")>]
    member this.Ops() =
        FSharp.toUnitTask this._Ops ()

    member private this._Ops() =
        async {
            let ctx = this.Context()
            let text = "```
 ██████╗ ██████╗ ███████╗██████╗ ██╗██████╗ ██╗
██╔═══██╗██╔══██╗██╔════╝╚════██╗██║╚════██╗██║
██║   ██║██████╔╝███████╗  ▄███╔╝██║  ▄███╔╝██║
██║   ██║██╔═══╝ ╚════██║  ▀▀══╝ ╚═╝  ▀▀══╝ ╚═╝
╚██████╔╝██║     ███████║  ██╗   ██╗  ██╗   ██╗
 ╚═════╝ ╚═╝     ╚══════╝  ╚═╝   ╚═╝  ╚═╝   ╚═╝
```"
            do! ctx.Channel.SendMessageAsync(text)
                |> Async.AwaitTask
                |> FSharp.ensureSuccess
        }

