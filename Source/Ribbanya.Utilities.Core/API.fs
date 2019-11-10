namespace Ribbanya.Utilities

open JetBrains.Annotations
open System.Runtime.CompilerServices

[<AbstractClass; Sealed; Extension; PublicAPI>]
type CSharpExtensions private () =
    [<Extension>]
    static member Swap(this: byref<_>, other: byref<_>) =
        let temp = this
        this <- other
        other <- temp
