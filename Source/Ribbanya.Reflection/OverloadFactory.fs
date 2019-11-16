namespace Ribbanya.Reflection.Emit

open System
open System.Linq.Expressions
open System.Reflection

module FSharpExtensions =
    let overload (method: MethodInfo) (overloadName: string) getDefaultParameter =
//        let getDefaultParameter : ParameterInfo -> _ =
//            match defaultParameters with
//            | Named named -> fun p -> Map.tryFind p.Name named
//            | Unnamed unnamed -> fun p -> Array.tryItem (p.Position - unnamed.StartOffset) unnamed.Parameters
        let folder (parameter: ParameterInfo) (overloadParameters, callParameters) =
            match getDefaultParameter parameter with
            | None ->
                let expr = Expression.Parameter(parameter.ParameterType, parameter.Name)
                expr :: overloadParameters, expr :> Expression :: callParameters
            | Some value ->
                let expr = Expression.Constant(value, parameter.ParameterType)
                overloadParameters, expr :> Expression :: callParameters
        
        let overloadParameters, callParameters = Array.foldBack folder (method.GetParameters()) ([],[])
        let call = Expression.Call(method, callParameters)
        
        Expression.Lambda(call, overloadName, true, overloadParameters).Compile()
//    type System.Linq.Expressions.Expression with
//        Lambda(Type, Expression, String, Boolean, IEnumerable<ParameterExpression>)	
//        Lambda(Type, Expression, String, IEnumerable<ParameterExpression>)	
//        Lambda(Type, Expression, Boolean, IEnumerable<ParameterExpression>)	
//        Lambda(Expression, String, Boolean, IEnumerable<ParameterExpression>)	
//        Lambda(Type, Expression, ParameterExpression[])	
//        Lambda(Type, Expression, Boolean, ParameterExpression[])	
//        Lambda(Expression, String, IEnumerable<ParameterExpression>)	
//        Lambda(Expression, Boolean, ParameterExpression[])	
//        Lambda(Expression, Boolean, IEnumerable<ParameterExpression>)	
//        Lambda(Type, Expression, IEnumerable<ParameterExpression>)	
//        Lambda(Expression, ParameterExpression[])	
//        Lambda(Expression, IEnumerable<ParameterExpression>)	
//        Lambda<TDelegate>(Expression, String, IEnumerable<ParameterExpression>)	
//        Lambda<TDelegate>(Expression, Boolean, ParameterExpression[])	
//        Lambda<TDelegate>(Expression, String, Boolean, IEnumerable<ParameterExpression>)	
//        Lambda<TDelegate>(Expression, ParameterExpression[])	
//        Lambda<TDelegate>(Expression, IEnumerable<ParameterExpression>)	
//        Lambda<TDelegate>(Expression, Boolean, IEnumerable<ParameterExpression>)