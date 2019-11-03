using System;
using Expressions.Expressions;
using Xunit;

namespace Expressions.Test.CsharpLanguage.ExpressionTests
{
    public class Casting : TestBase
    {
        [Fact]
        public void CastWithBuiltInFloat()
        {
            Resolve(
                "(float)7",
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
                "(float)(1 + 2)",
                new Cast(
                    new UnaryExpression(
                        new BinaryExpression(
                            new Constant(1),
                            new Constant(2),
                            ExpressionType.Add,
                            typeof(int)
                        ),
                        typeof(int),
                        ExpressionType.Group
                    ),
                    typeof(float)
                )
            );
        }

        [Fact]
        public void CastingWithFullType()
        {
            Resolve(
                "(System.Single)7",
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
                "(string[])null",
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
                "(string[,])null",
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
                "(string[,,])null",
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
                "(System.String[])null",
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
                "(System.String[,])null",
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
                "(System.String[,,])null",
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
                Resolve("(Unknown.Type)null");
            });
        }
    }
}
