using Xunit;

namespace Expressions.Test.CsharpLanguage.Compilation
{
    public class Fields : TestBase
    {
        [Fact]
        public void StaticFieldOnOwner()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "StaticField", 8);
        }

        [Fact]
        public void FieldOnOwner()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "Field", 7);
        }

        [Fact]
        public void ConstantAccess()
        {
            var context = new ExpressionContext(new[] { new Import("int", typeof(int)) });

            Resolve(context, "int.MaxValue", int.MaxValue);
        }

        public class Owner
        {
            public static int StaticField = 8;

            public int Field = 7;
        }
    }
}
