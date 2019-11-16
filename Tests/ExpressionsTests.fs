namespace Ribbanya.Tests

open System
open System.Linq.Expressions
open Xunit

[<Sealed>]
type ExpressionTests() =
    static member Abcdef a b c d e f =
        a ^^^ b ^^^ c ^^^ d ^^^ e ^^^ f
    static member Abc456 a b c = ExpressionTests.Abcdef a b c 4 5 6
    [<Fact>]
    member __.ExprTest() =
        Assert.Equal(ExpressionTests.Abcdef 1 2 3 4 5 6, ExpressionTests.Abc456 1 2 3)
        
        let method = typeof<ExpressionTests>.GetMethod("Abcdef")
        
        let abc456 =
            
            let a, b, c = Expression.Parameter(typeof<int>, "a"),
                          Expression.Parameter(typeof<int>, "b"),
                          Expression.Parameter(typeof<int>, "c")
            Expression.Lambda(
                Expression.Call(
                    method,
                        a, b, c,
                        Expression.Constant(box 4, typeof<int>),
                        Expression.Constant(box 5, typeof<int>),
                        Expression.Constant(box 6, typeof<int>)
                        ),
                    a, b, c).Compile() :?> System.Func<int,int,int,int>
            |> FuncConvert.FromFunc
        Assert.Equal(ExpressionTests.Abc456 1 2 3, abc456 1 2 3)