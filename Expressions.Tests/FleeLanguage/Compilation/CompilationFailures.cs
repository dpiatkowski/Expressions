using System;
using Xunit;

namespace Expressions.Test.FleeLanguage.Compilation
{
    public class CompilationFailures : TestBase
    {
        [Fact]
        public void InfiniteLoop()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                Resolve("true && false");
            });
        }
    }
}
