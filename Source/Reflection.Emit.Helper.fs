namespace Ribbanya.Reflection.Emit

open System
open System.Reflection.Emit

module internal Helper =
    let OperandTypeSize operandType =
        match operandType with
        | OperandType.InlineNone -> 0
        | OperandType.ShortInlineI
        | OperandType.ShortInlineVar -> sizeof<byte>
        | OperandType.InlineI -> sizeof<int>
        | OperandType.InlineI8 -> sizeof<int64>
        | OperandType.ShortInlineR -> sizeof<float32>
        | OperandType.InlineR -> sizeof<float>
        | OperandType.InlineMethod
        | OperandType.InlineString
        | OperandType.InlineType -> UIntPtr.Size
        | _ -> raise (NotImplementedException("Unexpected operand type " + operandType.ToString() + "."))