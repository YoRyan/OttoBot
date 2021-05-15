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
            let b64 = "IOKWiOKWiOKWiOKWiOKWiOKWiOKVlyDilojilojilojilojilojilojilZcg4paI4paI4paI4paI4paI4paI4paI4pWX4paI4paI4paI4paI4paI4paI4pWXIOKWiOKWiOKVl+KWiOKWiOKWiOKWiOKWiOKWiOKVlyDilojilojilZcK4paI4paI4pWU4pWQ4pWQ4pWQ4paI4paI4pWX4paI4paI4pWU4pWQ4pWQ4paI4paI4pWX4paI4paI4pWU4pWQ4pWQ4pWQ4pWQ4pWd4pWa4pWQ4pWQ4pWQ4pWQ4paI4paI4pWX4paI4paI4pWR4pWa4pWQ4pWQ4pWQ4pWQ4paI4paI4pWX4paI4paI4pWRCuKWiOKWiOKVkSAgIOKWiOKWiOKVkeKWiOKWiOKWiOKWiOKWiOKWiOKVlOKVneKWiOKWiOKWiOKWiOKWiOKWiOKWiOKVlyAg4paE4paI4paI4paI4pWU4pWd4paI4paI4pWRICDiloTilojilojilojilZTilZ3ilojilojilZEK4paI4paI4pWRICAg4paI4paI4pWR4paI4paI4pWU4pWQ4pWQ4pWQ4pWdIOKVmuKVkOKVkOKVkOKVkOKWiOKWiOKVkSAg4paA4paA4pWQ4pWQ4pWdIOKVmuKVkOKVnSAg4paA4paA4pWQ4pWQ4pWdIOKVmuKVkOKVnQrilZrilojilojilojilojilojilojilZTilZ3ilojilojilZEgICAgIOKWiOKWiOKWiOKWiOKWiOKWiOKWiOKVkSAg4paI4paI4pWXICAg4paI4paI4pWXICDilojilojilZcgICDilojilojilZcKIOKVmuKVkOKVkOKVkOKVkOKVkOKVnSDilZrilZDilZ0gICAgIOKVmuKVkOKVkOKVkOKVkOKVkOKVkOKVnSAg4pWa4pWQ4pWdICAg4pWa4pWQ4pWdICDilZrilZDilZ0gICDilZrilZDilZ0=";
            let text = Encoding.UTF8.GetString (Convert.FromBase64String b64)
            let ctx = this.Context()
            do! ctx.Channel.SendMessageAsync($"```{text}```")
                |> Async.AwaitTask
                |> FSharp.ensureSuccess
        }

