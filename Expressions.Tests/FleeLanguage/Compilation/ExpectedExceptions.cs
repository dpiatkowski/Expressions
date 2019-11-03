using System;
using System.Collections;
using Xunit;

namespace Expressions.Test.FleeLanguage.Compilation
{
    public class ExpectedExceptions : TestBase
    {
        [Fact]
        public void IntAndBoolSubtraction()
        {
            Resolve("1 - True", ExpressionsExceptionType.TypeMismatch);
        }

        [Fact]
        public void LongAndUlongEquals()
        {
            Resolve("ulong.minvalue = long.minvalue", ExpressionsExceptionType.TypeMismatch);
        }

        [Fact]
        public void StringAndString()
        {
            Resolve("\"abc\" and \"def\"", ExpressionsExceptionType.TypeMismatch);
        }

        [Fact]
        public void StringResultTypeForInt()
        {
            Resolve("100", ExpressionsExceptionType.InvalidExplicitCast, new BoundExpressionOptions
            {
                ResultType = typeof(string)
            });
        }

        [Fact]
        public void IntResultTypeForString()
        {
            Resolve("\"a\"", ExpressionsExceptionType.InvalidExplicitCast, new BoundExpressionOptions
            {
                ResultType = typeof(int)
            });
        }

        [Fact]
        public void StringResultTypeForType()
        {
            var context = new ExpressionContext();

            context.Variables.Add("a", typeof(string));

            Resolve(context, "a", ExpressionsExceptionType.TypeMismatch, new BoundExpressionOptions
            {
                ResultType = typeof(string)
            });
        }

        [Fact]
        public void UnresolvedIdentifier()
        {
            Resolve("FakeField + 1", ExpressionsExceptionType.UndefinedName);
        }

        [Fact]
        public void ReturnBuiltInType()
        {
            Resolve("String", ExpressionsExceptionType.TypeMismatch);
        }

        [Fact]
        public void UnresolvedMethod()
        {
            Resolve("Method()", ExpressionsExceptionType.UndefinedName);
        }

        [Fact]
        public void UnresolvedType()
        {
            Resolve("cast(1, UnknownType)", ExpressionsExceptionType.InvalidExplicitCast);
        }

        [Fact]
        public void IllegalValueCast()
        {
            Resolve("cast(\"a\", boolean)", ExpressionsExceptionType.InvalidExplicitCast);
        }

        [Fact]
        public void IllegalReferenceCast()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "cast(ICollectionA, Guid)", ExpressionsExceptionType.InvalidExplicitCast);
        }

        [Fact]
        public void SwallowedLexerErrors()
        {
            Resolve("\"\\z\"", ExpressionsExceptionType.SyntaxError);
        }

        [Fact]
        public void BoolEqualsNull()
        {
            Resolve("true = null", ExpressionsExceptionType.TypeMismatch);
        }

        [Fact]
        public void IntToString()
        {
            var context = new ExpressionContext();

            context.Variables.Add("stringA", "Hi");

            Resolve(context, "stringA.length", ExpressionsExceptionType.InvalidExplicitCast, new BoundExpressionOptions
            {
                ResultType = typeof(string)
            });
        }

        private void Resolve(string expression, ExpressionsExceptionType exceptionType)
        {
            Resolve(expression, exceptionType, null);
        }

        private void Resolve(string expression, ExpressionsExceptionType exceptionType, BoundExpressionOptions options)
        {
            Resolve(null, expression, exceptionType, options);
        }

        private void Resolve(ExpressionContext context, string expression, ExpressionsExceptionType exceptionType)
        {
            Resolve(context, expression, exceptionType, null);
        }

        private void Resolve(ExpressionContext context, string expression, ExpressionsExceptionType exceptionType, BoundExpressionOptions options)
        {
            try
            {
                Resolve(context, expression, null, options);
                Assert.True(false, string.Format("Expected exception type '{0}'", exceptionType));
            }
            catch (ExpressionsException ex)
            {
                Assert.Equal(exceptionType, ex.Type);
            }
            catch (Exception ex)
            {
                Assert.True(false, string.Format("Expected ExpressionsException got '{0}'", ex.GetType()));
            }
        }

        public class Owner
        {
            public ICollection ICollectionA { get; set; }

            public Owner()
            {
                ICollectionA = new ArrayList();
            }
        }
    }
}
