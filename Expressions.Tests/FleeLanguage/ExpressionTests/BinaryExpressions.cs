﻿using Expressions.Expressions;
using Xunit;

namespace Expressions.Test.FleeLanguage.ExpressionTests
{
    public class BinaryExpressions : TestBase
    {
        [Fact]
        public void BinaryTypeUnchanged()
        {
            Resolve(
                "1 + 2",
                new BinaryExpression(
                    new Constant(1),
                    new Constant(2),
                    ExpressionType.Add,
                    typeof(int)
                )
            );
        }

        [Fact]
        public void BinaryAddWithOneString()
        {
            Resolve(
                "1 + \"2\"",
                new MethodCall(
                    new TypeAccess(typeof(string)),
                    typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) }),
                    new IExpression[]
                    {
                        new Constant(1),
                        new Constant("2")
                    }
                )
            );
        }

        [Fact]
        public void LegalCasting()
        {
            Resolve(
                "1l + 1",
                new BinaryExpression(
                    new Constant(1L),
                    new Constant(1),
                    ExpressionType.Add,
                    typeof(long)
                )
            );
        }

        [Fact]
        public void LogicalAnd()
        {
            Resolve(
                "true and false",
                new BinaryExpression(
                    new Constant(true),
                    new Constant(false),
                    ExpressionType.And,
                    typeof(bool)
                )
            );
        }

        [Fact]
        public void LogicalOr()
        {
            Resolve(
                "true or false",
                new BinaryExpression(
                    new Constant(true),
                    new Constant(false),
                    ExpressionType.Or,
                    typeof(bool)
                )
            );
        }

        [Fact]
        public void LogicalXor()
        {
            Resolve(
                "true xor false",
                new BinaryExpression(
                    new Constant(true),
                    new Constant(false),
                    ExpressionType.Xor,
                    typeof(bool)
                )
            );
        }

        [Fact]
        public void Calculation()
        {
            Resolve(
                "2147483648U / 2u",
                new BinaryExpression(
                    new Constant(2147483648u),
                    new Constant(2u),
                    ExpressionType.Divide,
                    typeof(uint)
                )
            );
        }

        [Fact]
        public void MultipleAdditions()
        {
            Resolve(
                "100 + .25 + 000.25 + 1.50",
                new BinaryExpression(
                    new BinaryExpression(
                        new BinaryExpression(
                            new Constant(100),
                            new Constant(0.25),
                            ExpressionType.Add,
                            typeof(double)
                        ),
                        new Constant(0.25),
                        ExpressionType.Add,
                        typeof(double)
                    ),
                    new Constant(1.5),
                    ExpressionType.Add,
                    typeof(double)
                )
            );
        }

        [Fact]
        public void CompareAndEquals()
        {
            Resolve(
                "10 > 2 = true",
                new BinaryExpression(
                    new BinaryExpression(
                        new Constant(10),
                        new Constant(2),
                        ExpressionType.Greater,
                        typeof(bool),
                        typeof(int)
                    ),
                    new Constant(true),
                    ExpressionType.Equals,
                    typeof(bool)
                )
            );
        }

        [Fact]
        public void ComparesWithLongCast()
        {
            Resolve(
                "-100 > 100U",
                new BinaryExpression(
                    new Constant(-100),
                    new Constant(100u),
                    ExpressionType.Greater,
                    typeof(bool),
                    typeof(long)
                )
            );
        }

        [Fact]
        public void MultipleBitwise()
        {
            Resolve(
                "123 and 100 or 1245 xor 80",
                new BinaryExpression(
                    new BinaryExpression(
                        new Constant(123),
                        new Constant(100),
                        ExpressionType.And,
                        typeof(int)
                    ),
                    new BinaryExpression(
                        new Constant(1245),
                        new Constant(80),
                        ExpressionType.Xor,
                        typeof(int)
                    ),
                    ExpressionType.Or,
                    typeof(int)
                )
            );
        }

        [Fact]
        public void PrecedenceOfNot()
        {
            Resolve(
                "not 1 > 100",
                new UnaryExpression(
                    new BinaryExpression(
                        new Constant(1),
                        new Constant(100),
                        ExpressionType.Greater,
                        typeof(bool),
                        typeof(int)
                    ),
                    typeof(bool),
                    ExpressionType.Not
                )
            );
        }

        [Fact]
        public void AndAndNot()
        {
            Resolve(
                "true and not false and true",
                new BinaryExpression(
                    new BinaryExpression(
                        new Constant(true),
                        new UnaryExpression(
                            new Constant(false),
                            typeof(bool),
                            ExpressionType.Not
                        ),
                        ExpressionType.And,
                        typeof(bool)
                    ),
                    new Constant(true),
                    ExpressionType.And,
                    typeof(bool)
                )
            );
        }
    }
}
