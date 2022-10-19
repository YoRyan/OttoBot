namespace OttoCompute

open System.IO

type public Interaction =
    | Respond of text: string
    | RespondWithFile of filename: string * stream: Stream
    | Defer
    | Followup of text: string
    | FollowupWithFile of filename: string * stream: Stream
