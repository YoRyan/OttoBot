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

    /// Combine two lists such that for each position, the higher of the two values is retained.
    /// If the list lengths do not match, the resulting length is equal to that of the longer list.
    let mergeHigher a b =
        let rec _mergeHigher
            (accum: list<'a>) (a: list<'a>) (b: list<'a>)
            : list<'a> =
            match (a, b) with
            | ([], [])             -> List.rev accum
            | (ah :: at, [])       -> _mergeHigher (ah :: accum) at []
            | ([], bh :: bt)       -> _mergeHigher (bh :: accum) [] bt
            | (ah :: at, bh :: bt) -> _mergeHigher ((max ah bh) :: accum) at bt
        _mergeHigher [] a b

    /// Combine two lists into a list of pairs, but if the list lengths are not equal, substitute
    /// in a given value for the missing positions in the shorter list.
    let zipLonger defa defb a b =
        let rec _zipLonger
            (accum: list<'a * 'b>) (defa: 'a) (defb: 'b) (a: list<'a>) (b: list<'b>)
            : list<'a * 'b> =
            match (a, b) with
            | ([], [])             -> List.rev accum
            | (ah :: at, [])       -> _zipLonger ((ah, defb) :: accum) defa defb at []
            | ([], bh :: bt)       -> _zipLonger ((defa, bh) :: accum) defa defb [] bt
            | (ah :: at, bh :: bt) -> _zipLonger ((ah, bh) :: accum) defa defb at bt
        _zipLonger [] defa defb a b

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