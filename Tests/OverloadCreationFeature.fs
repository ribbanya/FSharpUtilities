namespace Ribbanya.Utilities.Tests

open JetBrains.Annotations
open Ribbanya.Reflection
open Ribbanya.Xbehave.Extensions.FSharpExtensions
open System
open System.Reflection
open Xbehave

type OverloadCreationFeature() =

    [<Scenario>]
    [<Exampleprivvodsdsadasdasdasdsadsadasdaffffffffffffdffffffaaa("APlusBTimesCMinusD")>]
    member __.``Overloads can be created from a static method and a list of parameters`` (methodName: string,
                                                                                          overloadName: string,
                                                                                          defaultParameters: obj [],
                                                                                          givenParameters: obj []) =
        let mutable method: MethodInfo = null
        "Given a method to overload".xi(fun () ->
            let flags = BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Static
            method <- typeof<OverloadCreationFeature>.GetMethod(methodName, flags))
        "And a name for the overload".x_
        "And some parameters to use as defaults".x_
        let mutable overload: Delegate = null
        "When an overload is generated as a delegate"
            .xi(fun () -> overload <- method.CreateOverload(overloadName, defaultParameters))

    [<UsedImplicitly>]
    static member internal APlusBTimesCMinusD(a: float, b: float, c: float, d: float): float = (a + b) * c - d

    [<UsedImplicitly>]
    static member internal APlusBTimes3Minus4(a: float, b: float): float =
        OverloadCreationFeature.APlusBTimesCMinusD(a, b, 3.0, 4.0)
