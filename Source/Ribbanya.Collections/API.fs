namespace Ribbanya.Collections

open JetBrains.Annotations
open System
open System.Linq
open System.Runtime.CompilerServices


[<AutoOpen>]
module private Helper =
    let identity = Func<_, _>(id)

[<RequireQualifiedAccess>]
module Seq =
    let findWithIndex predicate (source: seq<_>) =
        let enum = source.GetEnumerator()

        let rec loop index =
            let current = enum.Current
            if not (enum.MoveNext()) then ArgumentException "Item was not found." |> Error
            else if predicate current then Ok(index, current)
            else loop (index + 1)
        loop 0

[<RequireQualifiedAccess>]
module Queue =

    let empty<'a> : 'a list * 'a list = ([], [])

    let isEmpty =
        function
        | ([], []) -> true
        | _ -> false

    let enqueue x (push, pop) = (pop, x :: push)

    let rec dequeue =
        function
        | ([], []) -> (None, empty)
        | (x :: xs, push) -> (Some x, (xs, push))
        | ([], push) -> dequeue (List.rev push, [])

[<AbstractClass; Sealed; Extension; PublicAPI>]
type CSharpExtensions private () =

    [<Extension>]
    static member IndexOf this value = this |> Seq.findIndex (fun item -> item.Equals(value))

    [<Extension>]
    static member EnumerableEquals this other =
        let orderedThis = Enumerable.OrderBy(this, identity)
        let orderedOther = Enumerable.OrderBy(other, identity)
        Enumerable.SequenceEqual(orderedThis, orderedOther)
