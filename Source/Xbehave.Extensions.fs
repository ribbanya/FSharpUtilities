namespace Ribbanya.Xbehave

module Extensions =
    module FSharpExtensions =
        type System.String with
            member this.x (body: unit -> unit) = Xbehave.StringExtensions.x (this, body)
            member this.xi (body: unit -> unit) = this.x body |> ignore
            member this.x_ = this.x (fun () -> ()) |> ignore
