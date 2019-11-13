namespace Ribbanya.Entitas

open DesperateDevs.Utils
open Entitas
open System
open System.Reflection

[<Flags>]
type PublicMemberFilters =
    | Any = 0
    | MustRead = 1
    | MustWrite = 2
    | MustReadWrite = 3

[<AutoOpen>]
module FSharpExtensions =
    type System.Type with
        member this.getPublicMemberInfo(?filter: PublicMemberFilters) =
            let filter = defaultArg filter PublicMemberFilters.Any
            let mustRead = filter.HasFlag PublicMemberFilters.MustRead
            let mustWrite = filter.HasFlag PublicMemberFilters.MustWrite
            let flags = BindingFlags.Instance ||| BindingFlags.Public

            let fields =
                let predicate (field: FieldInfo) = not mustWrite || (not field.IsInitOnly && not field.IsLiteral)
                this.GetFields flags
                |> Array.filter predicate
                |> Array.map PublicMemberInfo
                
            let properties =
                let predicate (property: PropertyInfo) =
                    property.GetIndexParameters().Length = 0
                                  && (not mustRead || property.CanRead && property.GetGetMethod() <> null)
                                  && (not mustWrite || property.CanWrite && property.GetSetMethod() <> null)
                this.GetProperties flags
                |> Array.filter predicate
                |> Array.map PublicMemberInfo
                
            fields |> Array.append properties

    type Entitas.IContext with
        member this.entities =
            match this.GetType().GetMethod("GetEntities", BindingFlags.Public ||| BindingFlags.Instance) with
            | null -> MethodAccessException() |> Error
            | _ as mi -> mi.Invoke(this, null) :?> IEntity[] |> Ok