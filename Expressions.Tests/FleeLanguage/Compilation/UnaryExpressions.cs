using Xunit;

namespace Expressions.Test.FleeLanguage.Compilation
{
    public class UnaryExpressions : TestBase
    {
        [Fact]
        public void Arithatics()
        {
            Resolve("+1", 1);
            Resolve("-1", -1);

            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Variable1") { Value = 1 });

            Resolve(context, "+Variable1", 1);
            Resolve(context, "-Variable1", -1);
        }

        [Fact]
        public void Logicals()
        {
            Resolve("not true", false);
        }
    }
}
