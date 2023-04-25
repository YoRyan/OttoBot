module OttoBot.Helpers

/// Combine two lists such that for each position, the higher of the two values is retained.
/// If the list lengths do not match, the resulting length is equal to that of the longer list.
let mergeHigher a b =
    let rec _mergeHigher (accum: list<'a>) (a: list<'a>) (b: list<'a>) : list<'a> =
        match (a, b) with
        | ([], []) -> List.rev accum
        | (ah :: at, []) -> _mergeHigher (ah :: accum) at []
        | ([], bh :: bt) -> _mergeHigher (bh :: accum) [] bt
        | (ah :: at, bh :: bt) -> _mergeHigher ((max ah bh) :: accum) at bt

    _mergeHigher [] a b

/// Combine two lists into a list of pairs, but if the list lengths are not equal, substitute
/// in a given value for the missing positions in the shorter list.
let zipLonger defa defb a b =
    let rec _zipLonger (accum: list<'a * 'b>) (defa: 'a) (defb: 'b) (a: list<'a>) (b: list<'b>) : list<'a * 'b> =
        match (a, b) with
        | ([], []) -> List.rev accum
        | (ah :: at, []) -> _zipLonger ((ah, defb) :: accum) defa defb at []
        | ([], bh :: bt) -> _zipLonger ((defa, bh) :: accum) defa defb [] bt
        | (ah :: at, bh :: bt) -> _zipLonger ((ah, bh) :: accum) defa defb at bt

    _zipLonger [] defa defb a b

type TableRow =
    | Separator
    | Data of fields: seq<string>

/// Draw a table for a Discord message using a code block with ASCII art.
let makeTable (hsep: string) (vsep: string) (rows: seq<TableRow>) : string =
    let widthsByRow =
        Seq.map
            (function
            | Separator -> []
            | Data(fields) -> Seq.toList (Seq.map String.length fields))
            rows

    let widths = List.reduce mergeHigher (Seq.toList widthsByRow)

    let render =
        function
        | Separator ->
            let total = List.sum widths + vsep.Length * (widths.Length - 1)

            String.replicate (total / hsep.Length) hsep
        | Data(fields) ->
            Seq.map (fun (s: string, w: int) -> s.PadRight(w)) (zipLonger "" 0 (Seq.toList fields) widths)
            |> String.concat vsep

    let ascii = Seq.map render rows |> String.concat "\n"
    $"```{ascii}```"
