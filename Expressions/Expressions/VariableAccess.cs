﻿using System;

namespace Expressions.Expressions
{
    internal class VariableAccess : IExpression
    {
        public Type Type { get; private set; }

        public int ParameterIndex { get; private set; }

        public VariableAccess(Type type, int parameterIndex)
        {
            Require.NotNull(type, "type");

            Type = type;
            ParameterIndex = parameterIndex;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.VariableAccess(this);
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VariableAccess(this);
        }
    }
}
