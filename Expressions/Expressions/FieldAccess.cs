﻿using System;
using System.Reflection;

namespace Expressions.Expressions
{
    internal class FieldAccess : IExpression
    {
        public Type Type
        {
            get { return FieldInfo.FieldType; }
        }

        public IExpression Operand { get; private set; }

        public FieldInfo FieldInfo { get; private set; }

        public FieldAccess(IExpression operand, FieldInfo fieldInfo)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(fieldInfo, "fieldInfo");

            Operand = operand;
            FieldInfo = fieldInfo;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.FieldAccess(this);
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.FieldAccess(this);
        }
    }
}
