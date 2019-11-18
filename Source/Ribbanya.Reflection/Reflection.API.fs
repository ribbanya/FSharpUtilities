namespace Ribbanya.Reflection

open JetBrains.Annotations
open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

module private Helper =
    [<Literal>]
    let memberFullNameDelimiter = "."

[<AutoOpen>]
module FSharpExtensions =
    let typeDefaults = ConcurrentDictionary<Type, obj>()

    let SimpleTypes =
        (HashSet<Type>
            ([| typeof<string>
                typeof<decimal>
                typeof<DateTime>
                typeof<DateTimeOffset>
                typeof<TimeSpan>
                typeof<Guid> |]))

    type Type with

        member this.DefaultValue =
            if this.IsValueType then typeDefaults.GetOrAdd(this, (Activator.CreateInstance: Type -> obj))
            else null

        static member GetDefaultValue() = unbox typeof<_>.DefaultValue
        member this.IsSimple =
            this.IsValueType || this.IsPrimitive || SimpleTypes.Contains this
            || Convert.GetTypeCode this <> TypeCode.Object

    type ParameterInfo with
        member this.IsCSharpOptional = this.IsOptional && this.HasDefaultValue
        member this.CSharpDefaultValue =
            if this.IsOptional then
                if this.HasDefaultValue then Some this.DefaultValue
                else Some this.ParameterType.DefaultValue
            else None

    type MemberInfo with
        member this.GetFullName(?delimiter: string) =
            let delimiter = defaultArg delimiter Helper.memberFullNameDelimiter
            let (name, declaringType) = (this.Name, this.DeclaringType)
            match declaringType with
            | null -> name
            | _ -> declaringType.ToString() + delimiter + name

open FSharpExtensions

[<AbstractClass; Sealed; PublicAPI>]
type ReflectionHelper =
    static member GetDefaultValue() = Type.GetDefaultValue()

[<AbstractClass; Sealed; Extension; PublicAPI>]
type CSharpExtensions private () =

    [<Extension>]
    static member GetFullName(this: MemberInfo,
                              [<Optional; DefaultParameterValue(Helper.memberFullNameDelimiter)>] delimiter: string) =
        this.GetFullName(delimiter)

    [<Extension>]
    static member GetDefaultValue(this: Type) = this.DefaultValue

    [<Extension>]
    static member IsActuallyOptional(this: ParameterInfo) = this.IsCSharpOptional

    [<Extension>]
    static member GetActualDefaultValue(this: ParameterInfo) =
        match this.CSharpDefaultValue with
        | Some default' -> default'
        | None -> this.ToString() |> sprintf "Parameter %s requires a value." |> invalidOp

    [<Extension>]
    static member IsSimple(this: Type) = this.IsSimple
