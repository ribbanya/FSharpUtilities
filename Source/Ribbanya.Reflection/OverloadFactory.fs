namespace Ribbanya.Reflection.Emit

open Ribbanya.Reflection.Emit.Types
open Ribbanya.Reflection.FSharpExtensions
open System
open System.Reflection

module internal OverloadFactoryInternal =
    type ResolvedParameterValue =
        | Value of obj
        | Unspecified of Type

    type DefinedParameter =
        {Position: int
         Attributes: ParameterAttributes
         Name: string}

//    let createOverload (method: MethodInfo) overloadName defaultParameters overloadParameterTypes =
//        let overload = DynamicMethod(overloadName, method.ReturnType, overloadParameterTypes, method.Module)
//        let baseParameters = method.GetParameters()
        
        
            
//        let streamSize, definedParameters, localTypes, body =
//            let rec loop remainingParameters index argS streamSize definedParameters localTypes localIndex body =
//                match remainingParameters with
//                | parameter :: remainingParameters ->
//                    let baseParameter = baseParameters.[index]
//                    match parameter with
//                    | Unspecified ->
//                        let definedParameter =
//                            {Position = int argS
//                             Attributes = ParameterAttributes.None
//                             Name = baseParameter.Name}

//                        let instruction = Ldarg_S.resolve argS
//                        loop remainingParameters (index + 1) (argS + 1uy) (streamSize + instruction.Size)
//                            (definedParameter :: definedParameters) localTypes localIndex (instruction :: body)
//                    | Value value ->
//                        let localType = baseParameter.ParameterType
//                        let streamSize, localTypes, instructions =
//                            match value with
//                            | null ->
//                                let localTypes = localType :: localTypes
//                                let addInstruction (size: int, instructions) (instruction: Instruction) =
//                                    (streamSize + instruction.Size, instruction :: instructions)

//                                let size, instructions =
//                                    [Ldloc_S.resolve localIndex
//                                     {OpCode = OpCodes.Initobj
//                                      Operand = Operand.Type localType}
//                                     Ldloca_S.resolve localIndex]
//                                    |> List.fold addInstruction (0, [])
//                                (streamSize + size, localTypes, instructions)
//                            | _ ->
//                                let nullableType = Nullable.GetUnderlyingType localType
//                                let instruction =
//                                    if localType = typeof<bool> then 
//                        raise (NotImplementedException())
//                        loop remainingParameters (index + 1) argS streamSize definedParameters localTypes
//                            (instructions :: body)
//                | [] ->
//                    let call =
//                        {OpCode = OpCodes.Call
//                         Operand = Method(method)}

//                    let ret =
//                        {OpCode = OpCodes.Ret
//                         Operand = None}

//                    streamSize, definedParameters |> List.rev, localTypes |> List.rev, ret :: call :: body |> List.rev
//            loop defaultParameters 0 1uy 0 [] [] 0uy []
//        definedParameters
//        |> List.iter
//            (fun definedParameter ->
//            overload.DefineParameter(definedParameter.Position, definedParameter.Attributes, definedParameter.Name)
//            |> ignore)
//        let generator = overload.GetILGenerator streamSize
//        localTypes |> List.iter (fun localType -> generator.DeclareLocal localType |> ignore)
//        body |> List.iter (fun instruction -> generator.Emit instruction)
//        overload


    let arrangeParameters baseParameters getKey isDefaultParameter getBaseParameter getDefaultParameter =
        let resolveParameter (i: int) =
            let key = getKey i
            let (baseParameter: ParameterInfo) = getBaseParameter key
            if isDefaultParameter key then
                let type'() = baseParameter.ParameterType
                let parameterDefault() = Value baseParameter.DefaultValue 
                let typeDefault() = type'().DefaultValue |> Value 
                match getDefaultParameter key with
                | ParameterValue.Value value -> Convert.ChangeType(value, type'()) |> Value
                | ParameterValue.Unspecified -> type'() |> Unspecified
                | ParameterDefault -> parameterDefault()
                | TypeDefault -> typeDefault()
                | ParameterOrTypeDefault ->
                    if baseParameter.IsCSharpOptional then parameterDefault()
                    else typeDefault()
            else Value baseParameter
        let arrangedParameters = Array.init (Array.length<ParameterInfo> baseParameters) resolveParameter
        
        let folder rp types =
            match rp with
            | Unspecified type' -> type' :: types
            | _ -> types
        
        arrangedParameters,
        Array.foldBack folder arrangedParameters [] |> List.toArray
        
        
    let arrangeUnnamedParameters baseParameters defaultParameters startOffset =
        //TODO: Argument validation
        let getKey i = i
        let isDefaultParameter i = i >= startOffset && i < startOffset + (Array.length defaultParameters)
        let getBaseParameter = Array.get baseParameters
        let getDefaultParameter i = defaultParameters.[i - startOffset]
        arrangeParameters baseParameters getKey isDefaultParameter getBaseParameter getDefaultParameter
    let arrangeNamedParameters baseParameters defaultParameters =
        //TODO: Argument validation
        let baseParameterKeys = baseParameters |> Array.map (fun (bp: ParameterInfo) -> bp.Name)
        let getKey i = baseParameterKeys.[i]
        let isDefaultParameter name = defaultParameters |> Map.containsKey name
        let baseParametersMap =
            baseParameters
            |> Array.map (fun bp -> bp.Name, bp)
            |> Map.ofArray
        let getParameter map name = Map.find name map
        let getBaseParameter = getParameter baseParametersMap
        let getDefaultParameter = getParameter defaultParameters        
        arrangeParameters baseParameters getKey isDefaultParameter getBaseParameter getDefaultParameter