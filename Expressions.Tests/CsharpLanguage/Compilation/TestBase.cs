using System;
using Xunit;

namespace Expressions.Test.CsharpLanguage.Compilation
{
    public abstract class TestBase
    {
        protected void Resolve(string expression)
        {
            Resolve(null, expression);
        }

        protected void Resolve(ExpressionContext expressionContext, string expression)
        {
            Resolve(expressionContext, expression, null);
        }

        protected void Resolve(string expression, object expected)
        {
            Resolve(expression, expected, null);
        }

        protected void Resolve(string expression, object expected, BoundExpressionOptions options)
        {
            Resolve(null, expression, expected, options);
        }

        protected void Resolve(ExpressionContext expressionContext, string expression, object expected)
        {
            Resolve(expressionContext, expression, expected, null);
        }

        protected void Resolve(ExpressionContext expressionContext, string expression, object expected, BoundExpressionOptions options)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var dynamicExpression = new DynamicExpression(
                expression,
                ExpressionLanguage.Csharp
            );

            var actual = dynamicExpression.Invoke(
                expressionContext ?? new ExpressionContext(),
                options
            );

            Assert.Equal(expected, actual);
        }
    }
}
