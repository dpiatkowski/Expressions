using System;
using Xunit;

namespace Expressions.Test
{
    public abstract class BulkTests : ExpressionTests
    {
        protected BulkTests(ExpressionLanguage language)
            : base(language)
        {
        }

        [Fact(DisplayName = "Expressions that should be valid")]
        public void TestValidExpressions()
        {
            MyCurrentContext = MyGenericContext;

            ProcessScriptTests("ValidExpressions.txt", DoTestValidExpressions);
        }

        [Fact(DisplayName = "Special construct expressions that should be valid")]
        public void TestSpecialConstructExpressions()
        {
            MyCurrentContext = MyGenericContext;

            ProcessScriptTests("SpecialConstructs.txt", DoTestValidExpressions);
        }

        [Fact(DisplayName = "Casts that should be valid")]
        public void TestValidCasts()
        {
            MyCurrentContext = MyValidCastsContext;
            ProcessScriptTests("ValidCasts.txt", DoTestValidExpressions);
        }

        [Fact(DisplayName = "Test our handling of checked expressions")]
        public void TestCheckedExpressions()
        {
            ProcessScriptTests("CheckedTests.txt", DoTestCheckedExpressions);
        }

        private void DoTestValidExpressions(string[] arr)
        {
            string typeName = string.Concat("System.", arr[0]);
            Type expressionType = Type.GetType(typeName, true, true);

            ExpressionContext context = MyCurrentContext;
            //context.Options.ResultType = expressionType;

            var expression = new DynamicExpression(arr[1], Language);

            DoTest(expression, context, arr[2], expressionType, ExpressionTests.TestCulture);
        }

        private void DoTestCheckedExpressions(string[] arr)
        {
            string expression = arr[0];
            bool @checked = bool.Parse(arr[1]);
            bool shouldOverflow = bool.Parse(arr[2]);

            var imports = new[]
            {
                new Import(typeof(Math))
           };

            ExpressionContext context = new ExpressionContext(imports, MyValidExpressionsOwner);

            try
            {
                var e = new DynamicExpression(expression, Language);
                e.Invoke(context, new BoundExpressionOptions
                {
                    Checked = @checked
                });

                Assert.False(shouldOverflow);
            }
            catch (OverflowException)
            {
                Assert.True(shouldOverflow);
            }
        }
    }
}
