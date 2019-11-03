﻿using System;
using Expressions.Expressions;
using Xunit;

namespace Expressions.Test.CsharpLanguage.ExpressionTests
{
    public class UnaryExpressions : TestBase
    {
        [Fact]
        public void UnaryPlus()
        {
            Resolve(
                "+1",
                new UnaryExpression(
                    new Constant(1),
                    typeof(int),
                    ExpressionType.Plus
                )
            );
        }

        [Fact]
        public void UnaryMinus()
        {
            Resolve(
                "-1",
                new Constant(-1)
            );
        }

        [Fact]
        public void IllegalUnaryPlus()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                Resolve("+\"\"");
            });

        }

        [Fact]
        public void IllegalUnaryMinus()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                Resolve("-\"\"");
            });
        }

        [Fact]
        public void UnaryNot()
        {
            Resolve(
                "!true",
                new UnaryExpression(
                    new Constant(true),
                    typeof(bool),
                    ExpressionType.LogicalNot
                )
            );
        }

        [Fact]
        public void IllegalUnaryNot()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                Resolve("!\"\"");
            });
        }
    }
}
