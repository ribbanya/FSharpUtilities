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
        member this.GetPublicMemberInfo(?filter: PublicMemberFilters) =
            let filter = defaultArg filter PublicMemberFilters.Any
            let mustRead = filter.HasFlag PublicMemberFilters.MustRead
            let mustWrite = filter.HasFlag PublicMemberFilters.MustWrite
            let flags = BindingFlags.Instance ||| BindingFlags.Public
            seq {
                yield! seq {
                           for field in this.GetFields flags do
                               if not mustWrite || (not field.IsInitOnly && not field.IsLiteral) then
                                   yield (PublicMemberInfo(field))
                       }
                yield! seq {
                           for property in this.GetProperties flags do
                               if property.GetIndexParameters().Length = 0
                                  && (not mustRead || property.CanRead && property.GetGetMethod() <> null)
                                  && (not mustWrite || property.CanWrite && property.GetSetMethod() <> null) then
                                   yield (PublicMemberInfo(property))
                       }
            }

    type Entitas.IContext with
        member this.entities =
            match this.GetType().GetMethod("GetEntities", BindingFlags.Public ||| BindingFlags.Instance) with
            | null -> raise (MethodAccessException())
            | _ as mi -> mi.Invoke(this, null) :?> IEntity[]