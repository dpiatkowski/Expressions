using Xunit;

namespace Expressions.Test.FleeLanguage.Compilation
{
    public class Caching : TestBase
    {
        [Fact]
        public void SameExpressionReuses()
        {
            Assert.Same(
                new DynamicExpression("1", ExpressionLanguage.Flee).Cached,
                new DynamicExpression("1", ExpressionLanguage.Flee).Cached
            );
        }

        [Fact]
        public void SameTypes()
        {
            var dynamicExpression = new DynamicExpression("Variable", ExpressionLanguage.Flee);

            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Variable") { Value = 1 });

            Assert.Same(
                dynamicExpression.Bind(context),
                dynamicExpression.Bind(context)
            );
        }

        [Fact]
        public void DifferentTypesDifferentCache()
        {
            var dynamicExpression = new DynamicExpression("Variable", ExpressionLanguage.Flee);

            var context1 = new ExpressionContext();

            context1.Variables.Add(new Variable("Variable") { Value = 1 });

            var context2 = new ExpressionContext();

            context2.Variables.Add(new Variable("Variable") { Value = 1d });

            Assert.NotSame(
                dynamicExpression.Bind(context1),
                dynamicExpression.Bind(context2)
            );
        }

        [Fact]
        public void UnusedDifferentTypesAreSame()
        {
            var dynamicExpression = new DynamicExpression("1", ExpressionLanguage.Flee);

            var context1 = new ExpressionContext();

            context1.Variables.Add(new Variable("Variable") { Value = 1 });

            var context2 = new ExpressionContext();

            context2.Variables.Add(new Variable("Variable") { Value = 1d });

            Assert.Same(
                dynamicExpression.Bind(context1),
                dynamicExpression.Bind(context2)
            );
        }
    }
}
