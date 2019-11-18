
namespace Ribbanya.Reflection.Emit
(*
open JetBrains.Annotations
open System.Reflection
open System.Reflection.Emit
open Types

module private Helper =
    let emit (generator: ILGenerator) (instruction: Instruction) =
        let opCode = instruction.OpCode
        match instruction.Operand with
        | I1 operand -> generator.Emit(opCode, operand)
        | U1 operand -> generator.Emit(opCode, operand)
        | I2 operand -> generator.Emit(opCode, operand)
        | I4 operand -> generator.Emit(opCode, operand)
        | I8 operand -> generator.Emit(opCode, operand)
        | R4 operand -> generator.Emit(opCode, operand)
        | R8 operand -> generator.Emit(opCode, operand)
        | String operand -> generator.Emit(opCode, operand)
        | Type operand -> generator.Emit(opCode, operand)
        | Constructor operand -> generator.Emit(opCode, operand)
        | Method operand -> generator.Emit(opCode, operand)
        | Label operand -> generator.Emit(opCode, operand)
        | Labels operand -> generator.Emit(opCode, operand)
        | Field operand -> generator.Emit(opCode, operand)
        | Signature operand -> generator.Emit(opCode, operand)
        | Local operand -> generator.Emit(opCode, operand)
        | None -> generator.Emit(opCode)

[<AutoOpen; PublicAPI>]
module FSharpExtensions =
    type Emit.OperandType with
        member this.Size = EmitHelper.operandTypeSize this

    type OpCode with
        member this.TotalSize = this.Size + this.OperandType.Size

    type ILGenerator with
        member this.Emit instruction = Helper.emit this instruction

//open FSharpExtensions

// TODO
//[<AbstractClass; Sealed; Extension; PublicAPI>]
//type CSharpExtensions private () =
//
//    [<Extension>]
//    static member GetSize(this: Emit.OperandType) = this.Size
//
//    [<Extension>]
//    static member GetTotalSize(this: Emit.OpCode) = this.TotalSize
//
//    [<Extension>]
//    static member Emit this instruction = Helper.emit this instruction
//
//    [<Extension>]
//    static member CreateOverload(this, overloadName, defaultParameters, startOffset): 'TDelegate :> Delegate =
//        raise (NotImplementedException())
//
//    //TODO: Validate unspecifiedParameterTypes against Invoke
//    //    let (paddedParameters, unspecifiedParameterTypes) = arrangeParameters (this, defaultParameters, startOffset)
//    //    let overload = createOverload (this, overloadName, paddedParameters, x unspecifiedParameterTypes)
//    //    unbox<'TDelegate> (overload.CreateDelegate(typeof<'TDelegate>))
//    [<Extension>]
//    static member CreateOverload(this, overloadName, defaultParameters, startOffset): Delegate =
//        raise (NotImplementedException())
//
//    //    TODO: Validate parameters
//    //    let (paddedParameters, unspecifiedParameterTypes) = arrangeParameters (this, defaultParameters, startOffset)
//    //    let overload = createOverload (this, overloadName, paddedParameters, unspecifiedParameterTypes)
//    //    overload.CreateDelegate(typeof<'TDelegate>)
//    [<Extension>]
//    static member CreateOverload(this, overloadName, defaultParameters) = raise (NotImplementedException())
//    TODO: Validate parameters
//    let (paddedParameters, unspecifiedParameterTypes) = arrangeParameters (this, defaultParameters, startOffset)
//    let overload = createOverload (this, overloadName, paddedParameters, unspecifiedParameterTypes)
//    overload.CreateDelegate(typeof<'TDelegate>)
//  static member CreateOverload(this: MethodInfo, overloadName: string, defaultParameters: obj [], startOffset: int): Delegate =
//    raise (NotImplementedException())
*)