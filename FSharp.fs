namespace OttoBot

open System
open System.Threading.Tasks


module FSharp =

    let ensureSuccess fn =
        async {
            let! res = Async.Catch fn
            match res with
            | Choice1Of2 _ -> ()
            | Choice2Of2 e -> raise e
        }

    let toUnitTask fn =
        fn >> Async.StartAsTask >> fun taskUnit -> taskUnit :> Task

    type FuncHelpers =

        static member ToUnitDelegate<'a> (comp: 'a -> Async<unit>) =
            Func<'a, Task>
                (
                    toUnitTask comp
                )
                
        static member ToUnitDelegate<'a, 'b> (comp: 'a * 'b -> Async<unit>) =
            Func<'a, 'b, Task>
                (
                    fun a b -> toUnitTask comp (a, b)
                )
            
        static member ToUnitDelegate<'a, 'b, 'c> (comp: 'a * 'b * 'c -> Async<unit>) =
            Func<'a, 'b, 'c, Task>
                (
                    fun a b c -> toUnitTask comp (a, b, c)
                )