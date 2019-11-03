using System;
using Xunit;

namespace Expressions.Test.FleeLanguage.Compilation
{
    public class Casting : TestBase
    {
        [Fact]
        public void CastWithBuiltInFloat()
        {
            Resolve("cast(7, single)", 7.0f);
        }

        [Fact]
        public void CastWithExpression()
        {
            Resolve("cast(1 + 2, single)", 3f);
        }

        [Fact]
        public void CastingWithBuiltInStringArray()
        {
            Resolve("cast(null, string[])", null);
        }

        [Fact]
        public void CastingWithBuiltInStringArrayRank2()
        {
            Resolve("cast(null, string[,])", null);
        }

        [Fact]
        public void CastingWithBuiltInStringArrayRank3()
        {
            Resolve("cast(null, string[,,])", null);
        }

        [Fact]
        public void CastingWithFullStringArray()
        {
            Resolve("cast(null, System.String[])", null);
        }

        [Fact]
        public void CastingWithFullStringArrayRank2()
        {
            Resolve("cast(null, System.String[,])", null);
        }

        [Fact]
        public void CastingWithFullStringArrayRank3()
        {
            Resolve("cast(null, System.String[,,])", null);
        }

        [Fact]
        public void CastWithBuiltInString()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                Resolve("cast(7, string)");
            });
        }

        [Fact]
        public void ImplicitCast()
        {
            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Owner") { Value = new Owner() });

            Resolve(
                context,
                "Owner",
                1.0,
                new BoundExpressionOptions
                {
                    ResultType = typeof(double)
                }
            );
        }

        [Fact]
        public void OperatorAdd()
        {
            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Owner") { Value = new Owner() });

            Resolve(
                context,
                "Owner + 7",
                7
            );
        }

        [Fact]
        public void LongNumber()
        {
            Resolve(
                "0x8000000000000000",
                9223372036854775808,
                new BoundExpressionOptions
                {
                    ResultType = typeof(ulong)
                }
            );
        }

        [Fact]
        public void LongSignedNumber()
        {
            Resolve(
                "-0x7fffffffffffffff",
                -9223372036854775807,
                new BoundExpressionOptions
                {
                    ResultType = typeof(long)
                }
            );
        }

        [Fact]
        public void MinusDecimal()
        {
            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Decimal") { Value = 100m });

            Resolve(
                context,
                "-Decimal",
                -100m
            );
        }

        [Fact]
        public void ValidByteCast()
        {
            var context = new ExpressionContext();

            context.Variables.Add("bytea", (byte)50);

            Resolve(context, "cast(bytea,byte)", (byte)50, new BoundExpressionOptions
            {
                ResultType = typeof(byte)
            });
        }

        public class Owner
        {
            public static implicit operator double(Owner value)
            {
                return 1.0;
            }

            public static int operator +(Owner owner, int value)
            {
                return value;
            }
        }
    }
}
