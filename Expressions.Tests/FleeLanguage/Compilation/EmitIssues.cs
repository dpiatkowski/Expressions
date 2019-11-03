using System;
using Xunit;

namespace Expressions.Test.FleeLanguage.Compilation
{
    public class EmitIssues : TestBase
    {
        [Fact]
        public void LongBranch()
        {
            Resolve("true or (1.0+2.0+3.0+4.0+5.0+6.0+7.0+8.0+9.0+10.0+11.0+12.0+13.0+14.0+15.0 > 10.0)", true);
        }

        [Fact]
        public void SpecialLoadOfStructFields()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("Mouse", typeof(Mouse)) }),
                "Mouse.shareddt.year",
                1
            );
        }

        [Fact]
        public void ReadOnlyStaticAccess()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("DateTime", typeof(DateTime)) }),
                "DateTime.MinValue.Year",
                DateTime.MinValue.Year
            );
        }

        [Fact]
        public void ReadOnlyInstanceAccess()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "MinValue.Year",
                DateTime.MinValue.Year
            );
        }

        [Fact]
        public void ReadOnlyFieldAsReturn()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("string", typeof(string)) }),
                "string.empty",
                String.Empty
            );
        }

        [Fact]
        public void Issue()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("Mouse", typeof(Mouse)) }, new ExpressionOwner()),
                "DateTimeA.GetType().Name",
                "DateTime",
                new BoundExpressionOptions
                {
                    AllowPrivateAccess = true
                }
            );
        }

        public class Owner
        {
            public readonly DateTime MinValue;

            public Owner()
            {
                MinValue = DateTime.MinValue;
            }
        }
    }
}
