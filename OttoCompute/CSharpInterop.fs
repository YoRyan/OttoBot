namespace OttoCompute

open System.IO

type public Interaction =
    | Response of text: string
    | ResponseWithFile of filename: string * stream: Stream
