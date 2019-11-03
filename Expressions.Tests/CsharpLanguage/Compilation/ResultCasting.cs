using Xunit;

namespace Expressions.Test.CsharpLanguage.Compilation
{
    public class ResultCasting : TestBase
    {
        [Fact]
        public void CastIntToLong()
        {
            Resolve(
                "1",
                1L,
                new BoundExpressionOptions { ResultType = typeof(long) }
            );
        }

        [Fact]
        public void CastCharToDecimal()
        {
            Resolve(
                "'a'",
                97m,
                new BoundExpressionOptions { ResultType = typeof(decimal) }
            );
        }
    }
}
