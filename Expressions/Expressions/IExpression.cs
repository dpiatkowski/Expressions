using System;

namespace Expressions.Expressions
{
    internal interface IExpression
    {
        Type Type { get; }

        void Accept(IExpressionVisitor visitor);

        T Accept<T>(IExpressionVisitor<T> visitor);
    }
}
