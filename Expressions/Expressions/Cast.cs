﻿using System;

namespace Expressions.Expressions
{
    internal class Cast : IExpression
    {
        public IExpression Operand { get; private set; }

        public Type Type { get; private set; }

        public CastType CastType { get; set; }

        public Cast(IExpression operand, Type type)
            : this(operand, type, CastType.Cast)
        {
        }

        public Cast(IExpression operand, Type type, CastType castType)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(type, "type");

            Operand = operand;
            Type = type;
            CastType = castType;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Cast(this);
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Cast(this);
        }
    }
}
