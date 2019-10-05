using System;
using System.Collections.Generic;
using System.Diagnostics;
using Expressions.Expressions;

namespace Expressions
{
    internal class ConversionVisitor : ExpressionVisitor
    {
        private readonly Resolver _resolver;

        public ConversionVisitor(Resolver resolver)
        {
            Require.NotNull(resolver, "resolver");

            _resolver = resolver;
        }

        public override IExpression BinaryExpression(BinaryExpression binaryExpression)
        {
            var left = binaryExpression.Left.Accept(this);
            var right = binaryExpression.Right.Accept(this);

            if (
                (
                    binaryExpression.ExpressionType == ExpressionType.Add &&
                    (left.Type == typeof(string) || right.Type == typeof(string))
                ) ||
                binaryExpression.ExpressionType == ExpressionType.Concat
            )
            {
                return _resolver.ResolveMethod(
                    new TypeAccess(typeof(string)),
                    "Concat",
                    FlattenConcatArguments(binaryExpression.Left, binaryExpression.Right)
                );
            }
            else if (binaryExpression.ExpressionType == ExpressionType.Power)
            {
                return _resolver.ResolveMethod(
                    new TypeAccess(typeof(Math)),
                    "Pow",
                    new[] { left, right }
                );
            }

            if (left == binaryExpression.Left && right == binaryExpression.Right)
                return binaryExpression;
            else
                return new BinaryExpression(left, right, binaryExpression.ExpressionType, binaryExpression.Type);
        }

        private IExpression[] FlattenConcatArguments(IExpression left, IExpression right)
        {
            var arguments = new List<IExpression>();

            FlattenConcatArguments(arguments, left);
            FlattenConcatArguments(arguments, right);

            return arguments.ToArray();
        }

        private void FlattenConcatArguments(List<IExpression> arguments, IExpression argument)
        {
            var binaryExpression = argument as BinaryExpression;

            if (
                binaryExpression != null &&
                binaryExpression.ExpressionType == ExpressionType.Add
            )
            {
                FlattenConcatArguments(arguments, binaryExpression.Left);
                FlattenConcatArguments(arguments, binaryExpression.Right);
            }
            else
            {
                arguments.Add(argument.Accept(this));
            }
        }

        public override IExpression UnaryExpression(UnaryExpression unaryExpression)
        {
            var operand = unaryExpression.Operand.Accept(this);

            // Coerce -(uint) to -(ulong)(uint)

            if (
                unaryExpression.ExpressionType == ExpressionType.Minus &&
                operand.Type == typeof(uint)
            )
                return new UnaryExpression(new Cast(operand, typeof(long)), typeof(long), ExpressionType.Minus);

            if (operand == unaryExpression.Operand)
                return unaryExpression;
            else
                return new UnaryExpression(operand, unaryExpression.Type, unaryExpression.ExpressionType);
        }

        public override IExpression Cast(Cast cast)
        {
            if (cast.CastType == CastType.Convert)
            {
                var constant = cast.Operand as Constant;

                if (TypeUtil.CanCastImplicitely(
                    cast.Operand.Type,
                    cast.Type,
                    constant != null && constant.Value == null
                ))
                    return new Cast(cast.Operand, cast.Type);

                string methodName = null;

                switch (Type.GetTypeCode(cast.Type))
                {
                    case TypeCode.Boolean: methodName = "ToBoolean"; break;
                    case TypeCode.Byte: methodName = "ToByte"; break;
                    case TypeCode.Char: methodName = "ToChar"; break;
                    case TypeCode.DateTime: methodName = "ToDate"; break;
                    case TypeCode.Decimal: methodName = "ToDecimal"; break;
                    case TypeCode.Double: methodName = "ToDouble"; break;
                    case TypeCode.Int32: methodName = "ToInteger"; break;
                    case TypeCode.Int64: methodName = "ToLong"; break;
                    case TypeCode.SByte: methodName = "ToSByte"; break;
                    case TypeCode.Int16: methodName = "ToShort"; break;
                    case TypeCode.Single: methodName = "ToSingle"; break;
                    case TypeCode.String: methodName = "ToString"; break;
                    case TypeCode.UInt32: methodName = "ToUInteger"; break;
                    case TypeCode.UInt64: methodName = "ToULong"; break;
                    case TypeCode.UInt16: methodName = "ToUShort"; break;
                }

                if (methodName != null)
                {
                    var method = _resolver.FindOperatorMethod(
                        methodName,
                        Array.Empty<Type>(),
                        null,
                        new[] { cast.Operand.Type }
                    );

                    Debug.Assert(method != null && method.ReturnType == cast.Type);

                    return new MethodCall(
                        new TypeAccess(method.DeclaringType),
                        method,
                        new[]
                        {
                            cast.Operand
                        }
                    );
                }
                else
                {
                    return new Cast(cast.Operand, cast.Type);
                }
            }

            return base.Cast(cast);
        }
    }
}
