using Xunit;

namespace Expressions.Test.FleeLanguage.Parsing
{
    public class Parsing
    {
        [Fact]
        public void ValidSyntaxCheck()
        {
            DynamicExpression.CheckSyntax(
                "1", ExpressionLanguage.Flee
            );
        }

        [Fact]
        //[ExpectedException]
        public void InvalidSyntaxCheck()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                DynamicExpression.CheckSyntax("?", ExpressionLanguage.Flee);
            });
        }
    }
}
