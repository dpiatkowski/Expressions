﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Expressions.Expressions;

namespace Expressions
{
    internal sealed class BoundExpression : IBoundExpression
    {
        private readonly DynamicSignature _compiledMethod;
        private readonly CachedDynamicExpression _dynamicExpression;
        private readonly int[] _parameterMap;

#if DEBUG
        // For unit testing
        internal IExpression ResolvedExpression { get; private set; }
#endif

        internal BoundExpression(CachedDynamicExpression dynamicExpression, Type ownerType, Import[] imports, Type[] identifierTypes, BoundExpressionOptions options)
        {
            Require.NotNull(dynamicExpression, "dynamicExpression");
            Require.NotNull(imports, "imports");
            Require.NotNull(identifierTypes, "identifierTypes");

            _dynamicExpression = dynamicExpression;

            _parameterMap = BuildParameterMap(ownerType, identifierTypes);

            var resolver = new Resolver(_dynamicExpression, ownerType, imports, identifierTypes, _parameterMap, options);

            var resolvedTree = resolver.Resolve(_dynamicExpression.ParseResult.RootNode);

#if DEBUG
            ResolvedExpression = resolvedTree;
#endif

            _compiledMethod = CompileExpression(resolvedTree, ownerType, imports, identifierTypes, options, resolver);
        }

        private int[] BuildParameterMap(Type ownerType, Type[] identifierTypes)
        {
            var parameterMap = new List<int>();

            if (ownerType != null)
                parameterMap.Add(-1);

            for (int i = 0; i < identifierTypes.Length; i++)
            {
                if (identifierTypes[i] != null)
                    parameterMap.Add(i);
            }

            return parameterMap.ToArray();
        }

        private DynamicSignature CompileExpression(IExpression expression, Type ownerType, Import[] imports, Type[] identifierTypes, BoundExpressionOptions options, Resolver resolver)
        {
            var method = new DynamicMethod(
                "DynamicMethod",
                typeof(object),
                new[] { typeof(object[]) },
                options.RestrictedSkipVisibility
            );

            var il = method.GetILGenerator();

            new Compiler(il, resolver).Compile(expression);

            return (DynamicSignature)method.CreateDelegate(typeof(DynamicSignature));
        }

        public object Invoke()
        {
            return Invoke(null);
        }

        public object Invoke(IExecutionContext executionContext)
        {
            bool hadExecutionContext = executionContext != null;

            if (executionContext == null)
            {
                executionContext = new ExpressionContext();
            }

            var parameters = new object[_parameterMap.Length];

            bool ignoreCase = !DynamicExpression.IsLanguageCaseSensitive(_dynamicExpression.Language);
            var identifiers = _dynamicExpression.ParseResult.Identifiers;

            for (int i = 0; i < parameters.Length; i++)
            {
                int index = _parameterMap[i];

                if (index == -1)
                {
                    if (!hadExecutionContext)
                    {
                        throw new ExpressionsException(
                            "An owner was expected but no execution context has been provided",
                            ExpressionsExceptionType.InvalidOperation
                        );
                    }

                    parameters[i] = executionContext.Owner;
                }
                else
                {
                    parameters[i] = executionContext.GetVariableValue(
                        identifiers[index].Name, ignoreCase
                    );
                }
            }

            return _compiledMethod(parameters);
        }

        private delegate object DynamicSignature(object[] parameters);
    }

    internal sealed class BoundExpression<T> : IBoundExpression<T>
    {
        private readonly IBoundExpression _outer;

        internal BoundExpression(IBoundExpression outer)
        {
            Require.NotNull(outer, "outer");

            _outer = outer;
        }

        public T Invoke()
        {
            return (T)_outer.Invoke();
        }

        public T Invoke(IExecutionContext executionContext)
        {
            return (T)_outer.Invoke(executionContext);
        }

        object IBoundExpression.Invoke()
        {
            return _outer.Invoke();
        }

        object IBoundExpression.Invoke(IExecutionContext executionContext)
        {
            return _outer.Invoke(executionContext);
        }
    }
}
