using Expressions.Expressions;
using Xunit;

namespace Expressions.Test.FleeLanguage.ExpressionTests
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
