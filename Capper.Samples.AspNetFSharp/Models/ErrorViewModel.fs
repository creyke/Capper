namespace Capper.Samples.AspNetFSharp

open System

type ErrorViewModel private () =
    member val RequestId : string = null with get, set

    member val ShowRequestId : bool = true with get, set
