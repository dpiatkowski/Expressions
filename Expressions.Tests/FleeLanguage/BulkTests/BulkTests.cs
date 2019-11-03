using System;
using Xunit;

namespace Expressions.Test.FleeLanguage.BulkTests
{
    public class BulkTests : Test.BulkTests
    {
        public BulkTests()
            : base(ExpressionLanguage.Flee)
        {
        }

        [Fact(DisplayName = "Expressions that should not be valid")]
        public void TestInvalidExpressions()
        {
            ProcessScriptTests("InvalidExpressions.txt", DoTestInvalidExpressions);
        }

        private void DoTestInvalidExpressions(string[] arr)
        {
            var reason = (ExpressionsExceptionType)Enum.Parse(typeof(ExpressionsExceptionType), arr[2], true);

            var options = new BoundExpressionOptions
            {
                ResultType = Type.GetType(arr[0], true, true),
                AllowPrivateAccess = true
            };

            AssertCompileException(arr[1], MyGenericContext, options, reason);
        }
    }
}
