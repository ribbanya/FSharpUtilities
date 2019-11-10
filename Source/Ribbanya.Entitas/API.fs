namespace Ribbanya.Entitas

open System
open System.Reflection
open DesperateDevs.Utils

[<Flags>]
type PublicMemberFilters =
    | Any = 0
    | MustRead = 1
    | MustWrite = 2
    | MustReadWrite = 3


module FSharpExtensions =
    type System.Type with
        member this.GetPublicMemberInfo(?filter: PublicMemberFilters) =
            let filter = defaultArg filter PublicMemberFilters.Any
            let mustRead = filter.HasFlag PublicMemberFilters.MustRead
            let mustWrite = filter.HasFlag PublicMemberFilters.MustWrite
            let flags = BindingFlags.Instance ||| BindingFlags.Public

            let getFields seed =
                let folder fields (field: FieldInfo) =
                    if not mustWrite || (not field.IsInitOnly && not field.IsLiteral) then
                        (PublicMemberInfo(field)) :: fields
                    else fields
                this.GetFields flags |> Array.fold folder seed

            let getProperties seed =
                let folder properties (property: PropertyInfo) =
                    if property.GetIndexParameters().Length = 0
                       && (not mustRead || property.CanRead && property.GetGetMethod() <> null)
                       && (not mustWrite || property.CanWrite && property.GetSetMethod() <> null) then
                        (PublicMemberInfo(property)) :: properties
                    else properties
                this.GetProperties flags |> Array.fold folder seed

            []
            |> getFields
            |> getProperties
