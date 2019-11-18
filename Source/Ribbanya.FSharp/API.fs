namespace Ribbanya.FSharp

[<AutoOpen>]
module FSharpOperators =
    let inline ( ||||> ) a b c d ``fun`` = ``fun`` a b c d
    let inline ( |||||> ) a b c d e ``fun`` = ``fun`` a b c d e
    let inline ( ||||||> ) a b c d e f ``fun`` = ``fun`` a b c d e f
    let inline ( |||||||> ) a b c d e f g ``fun`` = ``fun`` a b c d e f g
    let inline ( <|||| ) ``fun`` a b c d = ``fun`` a b c d
    let inline ( <||||| ) ``fun`` a b c d e = ``fun`` a b c d e
    let inline ( <|||||| ) ``fun`` a b c d e f = ``fun`` a b c d e f
    let inline ( <||||||| ) ``fun`` a b c d e f g = ``fun`` a b c d e f g

    let inline ( *> ) (a) ``fun`` = ``fun``(a)
    let inline ( **> ) (a, b) ``fun`` = ``fun``(a, b)
    let inline ( ***> ) (a, b, c) ``fun`` = ``fun``(a, b, c)
    let inline ( ****> ) (a, b, c, d) ``fun`` = ``fun``(a, b, c, d)
    let inline ( *****> ) (a, b, c, d, e) ``fun`` = ``fun``(a, b, c, d, e)
    let inline ( ******> ) (a, b, c, d, e, f) ``fun`` = ``fun``(a, b, c, d, e, f)
    let inline ( *******> ) (a, b, c, d, e, f, g) ``fun`` = ``fun``(a, b, c, d, e, f, g)
    let inline ( <* ) ``fun`` (a) = ``fun``(a)
    let inline ( <** ) ``fun`` (a, b) = ``fun``(a, b)
    let inline ( <*** ) ``fun`` (a, b, c) = ``fun``(a, b, c)
    let inline ( <**** ) ``fun`` (a, b, c, d) = ``fun``(a, b, c, d)
    let inline ( <***** ) ``fun`` (a, b, c, d, e) = ``fun``(a, b, c, d, e)
    let inline ( <****** ) ``fun`` (a, b, c, d, e, f) = ``fun``(a, b, c, d, e, f)
    let inline ( <******* ) ``fun`` (a, b, c, d, e, f, g) = ``fun``(a, b, c, d, e, f, g)
