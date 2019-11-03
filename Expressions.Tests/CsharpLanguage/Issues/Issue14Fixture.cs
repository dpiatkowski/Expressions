using Expressions.Test.CsharpLanguage.ExpressionTests;
using Xunit;

namespace Expressions.Test.CsharpLanguage.Issues
{
    public class Issue14Fixture : TestBase
    {
        [Fact]
        public void Test()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                string translatedString = "uppercase(\"hello\")";
                var expr = new DynamicExpression(translatedString, ExpressionLanguage.Csharp);
                var context = new ExpressionContext(null, new CustomOwner(), true);
                var boundExpression = expr.Bind(context);
                object res = boundExpression.Invoke();
            });
        }

        public class CustomOwner
        {
            public string uppercase(string str)
            {
                return str.ToUpperInvariant();
            }
        }
    }
}
