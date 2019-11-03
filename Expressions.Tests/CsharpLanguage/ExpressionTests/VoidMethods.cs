using System;
using Xunit;

namespace Expressions.Test.CsharpLanguage.ExpressionTests
{
    public class VoidMethods : TestBase
    {
        [Fact]
        public void VoidMethodsAreInvisible()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                Resolve(new ExpressionContext(null, new Owner()), "VoidMethod()");
            });
        }

        public class Owner
        {
            public void VoidMethod()
            {
            }
        }
    }
}
