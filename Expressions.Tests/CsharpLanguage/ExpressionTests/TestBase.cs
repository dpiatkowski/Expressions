﻿using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using Xunit;

namespace Expressions.Test.CsharpLanguage.ExpressionTests
{
    public abstract class TestBase
    {
        internal void Resolve(string expression)
        {
            Resolve(null, expression);
        }

        internal void Resolve(ExpressionContext expressionContext, string expression)
        {
            Resolve(expressionContext, expression, null);
        }

        internal void Resolve(string expression, IExpression expectedResult)
        {
            Resolve(null, expression, expectedResult);
        }

        internal void Resolve(ExpressionContext expressionContext, string expression, IExpression expectedResult)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var boundExpression = new DynamicExpression(
                expression,
                ExpressionLanguage.Csharp
            ).Bind(
                expressionContext ?? new ExpressionContext()
            );

            string expected = new ExpressionPrinter(expectedResult).ToString();
            string actual = new ExpressionPrinter(((BoundExpression)boundExpression).ResolvedExpression).ToString();

            // TODO: Implement equals.

            if (expected != actual)
            {
                Console.WriteLine(PrintSideToSide("Expected:\r\n" + expected, "Actual:\r\n" + actual));

                Assert.Equal(expected, actual);
            }
        }

        private string PrintSideToSide(string left, string right)
        {
            var sb = new StringBuilder();

            var leftLines = GetLines(left);
            var rightLines = GetLines(right);

            int longestLeft = int.MinValue;

            foreach (string line in leftLines)
            {
                longestLeft = Math.Max(longestLeft, line.TrimEnd().Length);
            }

            for (int i = 0; i < Math.Max(leftLines.Length, rightLines.Length); i++)
            {
                string leftLine = i >= leftLines.Length ? "" : leftLines[i];
                string rightLine = i >= rightLines.Length ? "" : rightLines[i];

                string separator = leftLine != rightLine ? " | " : "   ";

                sb.Append(leftLine);
                sb.Append(new string(' ', longestLeft - leftLine.Length));
                sb.Append(separator);
                sb.AppendLine(rightLine);
            }

            return sb.ToString();
        }

        private string[] GetLines(string text)
        {
            var lines = new List<string>();

            foreach (string line in text.Split('\n'))
            {
                lines.Add(line.TrimEnd());
            }

            return lines.ToArray();
        }
    }
}
