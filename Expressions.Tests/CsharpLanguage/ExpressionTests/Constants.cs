using Expressions.Expressions;
using Xunit;

namespace Expressions.Test.CsharpLanguage.ExpressionTests
{
    public class Constants : TestBase
    {
        [Fact]
        public void Max()
        {
            Resolve(
                int.MinValue.ToString(),
                new Constant(int.MinValue)
            );
        }
    }
}
