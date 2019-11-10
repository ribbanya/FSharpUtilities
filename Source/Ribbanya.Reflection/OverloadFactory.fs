namespace Ribbanya.Reflection.Emit

open JetBrains.Annotations
open Ribbanya.Reflection.Emit.FSharpExtensions
open Ribbanya.Reflection.Emit.Types
open Ribbanya.Reflection.FSharpExtensions
open System
open System
open System.Reflection
open System.Reflection.Emit
open System.Runtime.CompilerServices

module internal OverloadFactoryInternal =
    type ResolvedParameterValue =
        | Value of obj
        | Unspecified

    type ParameterKey =
        | Position of int
        | Name of string

    type DefinedParameter =
        {Position: int
         Attributes: ParameterAttributes
         Name: string}

    let createOverload (method: MethodInfo) overloadName defaultParameters overloadParameterTypes =
        let overload = new DynamicMethod(overloadName, method.ReturnType, overloadParameterTypes, method.Module)
        let baseParameters = method.GetParameters()

        let streamSize, definedParameters, localTypes, body =
            let rec loop remainingParameters index argS streamSize definedParameters localTypes localIndex body =
                match remainingParameters with
                | parameter :: remainingParameters ->
                    let baseParameter = baseParameters.[index]
                    match parameter with
                    | Unspecified ->
                        let definedParameter =
                            {Position = int argS
                             Attributes = ParameterAttributes.None
                             Name = baseParameter.Name}

                        let instruction = Ldarg_S.resolve argS
                        loop remainingParameters (index + 1) (argS + 1uy) (streamSize + instruction.Size)
                            (definedParameter :: definedParameters) localTypes localIndex (instruction :: body)
                    | Value value ->
                        let localType = baseParameter.ParameterType
                        let streamSize, localTypes, instructions =
                            match value with
                            | null ->
                                let localTypes = localType :: localTypes
                                let addInstruction (size: int, instructions) (instruction: Instruction) =
                                    (streamSize + instruction.Size, instruction :: instructions)

                                let size, instructions =
                                    [Ldloc_S.resolve localIndex
                                     {OpCode = OpCodes.Initobj
                                      Operand = Operand.Type localType}
                                     Ldloca_S.resolve localIndex]
                                    |> List.fold addInstruction (0, [])
                                (streamSize + size, localTypes, instructions)
                            | _ ->
                                let nullableType = Nullable.GetUnderlyingType localType
                                let instruction =
                                    if localType = typeof<bool> then 
                        raise (NotImplementedException())
                        loop remainingParameters (index + 1) argS streamSize definedParameters localTypes
                            (instructions :: body)
                | [] ->
                    let call =
                        {OpCode = OpCodes.Call
                         Operand = Method(method)}

                    let ret =
                        {OpCode = OpCodes.Ret
                         Operand = None}

                    streamSize, definedParameters |> List.rev, localTypes |> List.rev, ret :: call :: body |> List.rev
            loop defaultParameters 0 1uy 0 [] [] 0uy []
        definedParameters
        |> List.iter
            (fun definedParameter ->
            overload.DefineParameter(definedParameter.Position, definedParameter.Attributes, definedParameter.Name)
            |> ignore)
        let generator = overload.GetILGenerator streamSize
        localTypes |> List.iter (fun localType -> generator.DeclareLocal localType |> ignore)
        body |> List.iter (fun instruction -> generator.Emit instruction)
        overload

    let unboxParameter (parameter: obj) =
        match parameter with
        | :? ParameterValue as parameter' -> parameter'
        | _ -> ParameterValue.Value(parameter)

    let arrangeParameters (baseParameters: Map<_, _>) (defaultParameters: Map<_, _>) =
        let paddedParameters = Array.create baseParameters.Count Unspecified

        let folder unspecifiedParameterTypes key (baseParameter: ParameterInfo) =
            let position = baseParameter.Position
            let type' = baseParameter.ParameterType
            if defaultParameters.ContainsKey(key) then
                let defaultParameter = defaultParameters.[key]

                let resolvedParameter =
                    let type' = baseParameter.ParameterType
                    match defaultParameter with
                    | ParameterValue.Value value -> Value(Convert.ChangeType(value, type'))
                    | ParameterValue.Unspecified -> Unspecified
                    | ParameterDefault -> Value(baseParameter.DefaultValue)
                    | TypeDefault -> Value(type'.DefaultValue)
                    | ParameterOrTypeDefault ->
                        if baseParameter.IsCSharpOptional then Value(baseParameter.DefaultValue)
                        else Value(type'.DefaultValue)
                paddedParameters.[position] <- resolvedParameter
                unspecifiedParameterTypes
            else type' :: unspecifiedParameterTypes
        paddedParameters,
        baseParameters
        |> Map.fold folder []
        |> List.rev
