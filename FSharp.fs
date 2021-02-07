namespace OttoBot

open System
open System.Threading.Tasks


module FSharp =

    /// Discard the return value, like '|> ignore', but raise exceptions instead of discarding them.
    let ensureSuccess fn =
        async {
            let! res = Async.Catch fn
            match res with
            | Choice1Of2 _ -> ()
            | Choice2Of2 e -> raise e
        }

    /// Cast an F# 'Async<unit>' to a C# 'Task'.
    let toUnitTask fn =
        fn >> Async.StartAsTask >> fun taskUnit -> taskUnit :> Task

    /// A library of helpers for casting between F# and C# functions.
    type Helpers =

        /// Cast an F# asynchronous function to a C# Task delegate.
        static member ToUnitDelegate<'a> (comp: 'a -> Async<unit>) =
            Func<'a, Task>
                (
                    toUnitTask comp
                )
                
        /// Cast an F# asynchronous function to a C# Task delegate.
        static member ToUnitDelegate<'a, 'b> (comp: 'a * 'b -> Async<unit>) =
            Func<'a, 'b, Task>
                (
                    fun a b -> toUnitTask comp (a, b)
                )
            
        /// Cast an F# asynchronous function to a C# Task delegate.
        static member ToUnitDelegate<'a, 'b, 'c> (comp: 'a * 'b * 'c -> Async<unit>) =
            Func<'a, 'b, 'c, Task>
                (
                    fun a b c -> toUnitTask comp (a, b, c)
                )