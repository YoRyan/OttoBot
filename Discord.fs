namespace OttoBot


module Discord =
    
    type TableRow =
    | Separator
    | Data of fields : seq<string>

    /// Draw a table for a Discord message using a code block with ASCII art.
    let makeTable (hsep: string) (vsep: string) (rows: seq<TableRow>) : string =
        let widthsByRow =
            Seq.map
                (
                    function
                    | Separator -> []
                    | Data(fields) -> Seq.toList (Seq.map String.length fields)
                )
                rows
        let widths = List.reduce FSharp.mergeHigher (Seq.toList widthsByRow)

        let render = function
            | Separator ->
                let total = List.sum widths + vsep.Length*(widths.Length - 1)
                String.replicate (total/hsep.Length) hsep
            | Data(fields) ->
                Seq.map
                    (fun (s: string, w: int) -> s.PadRight(w))
                    (FSharp.zipLonger "" 0 (Seq.toList fields) widths)
                |> String.concat vsep

        let ascii = Seq.map render rows |> String.concat "\n"
        $"```{ascii}```"