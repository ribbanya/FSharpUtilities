namespace Ribbanya.Reflection.Emit

open System
open System.Reflection
open System.Reflection.Emit

module Types =
    type Operand =
        | I1 of sbyte
        | U1 of byte
        | I2 of int16
        | I4 of int
        | I8 of int64
        | R4 of float32
        | R8 of float
        | String of string
        | Type of Type
        | Constructor of ConstructorInfo
        | Method of MethodInfo
        | Label of Label
        | Labels of Label []
        | Field of FieldInfo
        | Signature of SignatureHelper
        | Local of LocalBuilder
        | None

        member this.OperandType =
            match this with
            | I1 _ -> OperandType.ShortInlineI
            | U1 _ -> OperandType.ShortInlineI
            | I2 _ -> OperandType.InlineI
            | I4 _ -> OperandType.InlineI
            | I8 _ -> OperandType.InlineI8
            | R4 _ -> OperandType.ShortInlineR
            | R8 _ -> OperandType.InlineR
            | String _ -> OperandType.InlineString
            | Type _ -> OperandType.InlineType
            | Constructor _ -> OperandType.InlineMethod
            | Method _ -> OperandType.InlineMethod
            | Label _ -> raise (NotImplementedException())
            | Labels _ -> raise (NotImplementedException())
            | Field _ -> OperandType.InlineField
            | Signature _ -> OperandType.InlineSig
            | Local _ -> raise (NotImplementedException())
            | None _ -> OperandType.InlineNone

        member this.Size = Helper.operandTypeSize this.OperandType

    type Instruction =
        {OpCode: OpCode
         Operand: Operand}
        member this.Size = this.OpCode.Size + Helper.operandTypeSize this.Operand.OperandType
            

    type ShortMacroInstructionType =
        | Ldarg_S
        | Ldarga_S
        | Ldloc_S
        | Ldloca_S
        member this.resolve (index: byte) =
            match this with
            | Ldarg_S ->
                match index with
                | 0uy ->
                    {OpCode = OpCodes.Ldarg_0
                     Operand = None}
                | 1uy ->
                    {OpCode = OpCodes.Ldarg_1
                     Operand = None}
                | 2uy ->
                    {OpCode = OpCodes.Ldarg_2
                     Operand = None}
                | 3uy ->
                    {OpCode = OpCodes.Ldarg_3
                     Operand = None}
                | _ ->
                    {OpCode = OpCodes.Ldarg_S
                     Operand = U1(index)}
            | Ldarga_S ->
                {OpCode = OpCodes.Ldarga_S
                 Operand = U1(index)}
            | Ldloc_S ->
                match index with
                | 0uy ->
                    {OpCode = OpCodes.Ldloc_0
                     Operand = None}
                | 1uy ->
                    {OpCode = OpCodes.Ldloc_1
                     Operand = None}
                | 2uy ->
                    {OpCode = OpCodes.Ldloc_2
                     Operand = None}
                | 3uy ->
                    {OpCode = OpCodes.Ldloc_3
                     Operand = None}
                | _ ->
                    {OpCode = OpCodes.Ldloc_S
                     Operand = U1(index)}
            | Ldloca_S ->
                {OpCode = OpCodes.Ldloca_S
                 Operand = U1(index)}

    type ParameterValue =
        | Value of obj
        | Unspecified
        | ParameterDefault
        | TypeDefault
        | ParameterOrTypeDefault
