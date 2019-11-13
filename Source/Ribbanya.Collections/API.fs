namespace Ribbanya.Collections

open System
open System.Runtime.CompilerServices
open System.Linq
open JetBrains.Annotations


[<AutoOpen>]
module private Helper =
    let identity = Func<_, _>(fun value -> value)

[<RequireQualifiedAccess>]
module Seq =
    let findWithIndex predicate (source: seq<_>) =
        let enum = source.GetEnumerator()
        let rec loop index =
            let current = enum.Current
            if not (enum.MoveNext()) then
                ArgumentException "Item was not found." |> Error
            else if predicate current then Ok (index, current)
            else loop (index + 1)
        loop 0

[<AbstractClass; Sealed; Extension; PublicAPI>]
type CSharpExtensions private () =

    [<Extension>]
    static member IndexOf this value = this |> Seq.findIndex (fun item -> item.Equals(value))

    [<Extension>]
    static member EnumerableEquals this other =
        let orderedThis = Enumerable.OrderBy(this, identity)
        let orderedOther = Enumerable.OrderBy(other, identity)
        Enumerable.SequenceEqual(orderedThis, orderedOther)
