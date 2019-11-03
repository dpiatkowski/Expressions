using System;
using Expressions.Expressions;
using Xunit;

namespace Expressions.Test.FleeLanguage.ExpressionTests
{
    public class Conversions : TestBase
    {
        [Fact]
        public void StringConcat()
        {
            Resolve(
                "\"a\" + \"a\"",
                new MethodCall(
                    new TypeAccess(typeof(string)),
                    typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }),
                    new IExpression[]
                    {
                        new Constant("a"),
                        new Constant("a")
                    }
                )
            );
        }

        [Fact]
        public void MultipleConcat()
        {
            Resolve(
                "\"a\" + \"a\" + \"a\" + \"a\" + \"a\" + \"a\"",
                new MethodCall(
                    new TypeAccess(typeof(string)),
                    typeof(string).GetMethod("Concat", new[] { typeof(string[]) }),
                    new IExpression[]
                    {
                        new Constant("a"),
                        new Constant("a"),
                        new Constant("a"),
                        new Constant("a"),
                        new Constant("a"),
                        new Constant("a")
                    }
                )
            );
        }

        [Fact]
        public void ConcatWithBoxing()
        {
            Resolve(
                "\"a\" + 1",
                new MethodCall(
                    new TypeAccess(typeof(string)),
                    typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) }),
                    new IExpression[]
                    {
                        new Constant("a"),
                        new Constant(1)
                    }
                )
            );
        }

        [Fact]
        public void Power()
        {
            Resolve(
                "1 ^ 2",
                new MethodCall(
                    new TypeAccess(typeof(Math)),
                    typeof(Math).GetMethod("Pow"),
                    new IExpression[]
                    {
                        new Constant(1),
                        new Constant(2)
                    }
                )
            );
        }
    }
}
