namespace Ribbanya.Collections

open JetBrains.Annotations
open System
open System.Linq
open System.Runtime.CompilerServices

[<AutoOpen>]
module private Helper =
    let identity = Func<_, _>(fun value -> value)

[<AbstractClass; Sealed; Extension; PublicAPI>]
type CSharpExtensions private () =

    [<Extension>]
    static member IndexOf this value = this |> Seq.findIndex (fun item -> item.Equals(value))

    [<Extension>]
    static member EnumerableEquals this other =
        let orderedThis = Enumerable.OrderBy(this, identity)
        let orderedOther = Enumerable.OrderBy(other, identity)
        Enumerable.SequenceEqual(orderedThis, orderedOther)
