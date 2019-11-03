using Xunit;

namespace Expressions.Test.CsharpLanguage.Parsing
{
    public class Parsing
    {
        [Fact]
        public void ValidSyntaxCheck()
        {
            DynamicExpression.CheckSyntax(
                "1", ExpressionLanguage.Csharp
            );
        }

        [Fact]
        public void InvalidSyntaxCheck()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                DynamicExpression.CheckSyntax("?", ExpressionLanguage.Csharp);
            });
        }
    }
}
