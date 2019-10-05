﻿using System;

namespace Expressions.Expressions
{
    internal class UnaryExpression : IExpression
    {
        public IExpression Operand { get; private set; }

        public Type Type { get; private set; }

        public ExpressionType ExpressionType { get; private set; }

        public UnaryExpression(IExpression operand, Type type, ExpressionType expressionType)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(type, "type");

            Operand = operand;
            Type = type;
            ExpressionType = expressionType;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.UnaryExpression(this);
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.UnaryExpression(this);
        }
    }
}
