using System;
using Expressions.Expressions;
using Xunit;

namespace Expressions.Test.FleeLanguage.ExpressionTests
{
    public class SpecialConstructs : TestBase
    {
        [Fact]
        public void MethodOnZeroConstant()
        {
            Resolve(
                "0.ToString()",
                new MethodCall(
                    new Constant(0),
                    typeof(int).GetMethod("ToString", new Type[0]),
                    new IExpression[0]
                )
            );
        }

        [Fact]
        public void MethodOnZerosConstant()
        {
            Resolve(
                "000.ToString()",
                new MethodCall(
                    new Constant(0),
                    typeof(int).GetMethod("ToString", new Type[0]),
                    new IExpression[0]
                )
            );
        }

        [Fact]
        public void MethodOnOneConstant()
        {
            Resolve(
                "1.ToString()",
                new MethodCall(
                    new Constant(1),
                    typeof(int).GetMethod("ToString", new Type[0]),
                    new IExpression[0]
                )
            );
        }
    }
}
