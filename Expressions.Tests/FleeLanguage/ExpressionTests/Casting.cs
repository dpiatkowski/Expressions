using System;
using Expressions.Expressions;
using Xunit;

namespace Expressions.Test.FleeLanguage.ExpressionTests
{
    public class Casting : TestBase
    {
        [Fact]
        public void CastWithBuiltInFloat()
        {
            Resolve(
                "cast(7, single)",
                new Cast(
                    new Constant(7),
                    typeof(float)
                )
            );
        }

        [Fact]
        public void CastWithExpression()
        {
            Resolve(
                "cast(1 + 2, single)",
                new Cast(
                    new BinaryExpression(
                        new Constant(1),
                        new Constant(2),
                        ExpressionType.Add,
                        typeof(int)
                    ),
                    typeof(float)
                )
            );
        }

        [Fact]
        public void CastingWithFullType()
        {
            Resolve(
                "cast(7, System.Single)",
                new Cast(
                    new Constant(7),
                    typeof(float)
                )
            );
        }

        [Fact]
        public void CastingWithBuiltInStringArray()
        {
            Resolve(
                "cast(null, string[])",
                new Cast(
                    new Constant(null),
                    typeof(string[])
                )
            );
        }

        [Fact]
        public void CastingWithBuiltInStringArrayRank2()
        {
            Resolve(
                "cast(null, string[,])",
                new Cast(
                    new Constant(null),
                    typeof(string[,])
                )
            );
        }

        [Fact]
        public void CastingWithBuiltInStringArrayRank3()
        {
            Resolve(
                "cast(null, string[,,])",
                new Cast(
                    new Constant(null),
                    typeof(string[,,])
                )
            );
        }

        [Fact]
        public void CastingWithFullStringArray()
        {
            Resolve(
                "cast(null, System.String[])",
                new Cast(
                    new Constant(null),
                    typeof(string[])
                )
            );
        }

        [Fact]
        public void CastingWithFullStringArrayRank2()
        {
            Resolve(
                "cast(null, System.String[,])",
                new Cast(
                    new Constant(null),
                    typeof(string[,])
                )
            );
        }

        [Fact]
        public void CastingWithFullStringArrayRank3()
        {
            Resolve(
                "cast(null, System.String[,,])",
                new Cast(
                    new Constant(null),
                    typeof(string[,,])
                )
            );
        }

        [Fact]
        public void CastingToUnknownType()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                Resolve("cast(null, Unknown.Type)");
            });
        }
    }
}
