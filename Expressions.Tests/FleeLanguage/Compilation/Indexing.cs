using Xunit;

namespace Expressions.Test.FleeLanguage.Compilation
{
    public class Indexing : TestBase
    {
        [Fact]
        public void ArrayIndexOnOwner()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "ArrayProperty[0]", 1);
        }

        [Fact]
        public void RankedArrayIndexOnOwner()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "RankedProperty[0,0]", 1);
            Resolve(context, "RankedProperty[1,1]", 4);
        }

        public class Owner
        {
            public int[] ArrayProperty
            {
                get { return new[] { 1, 2, 3 }; }
            }

            public int[,] RankedProperty
            {
                get
                {
                    return new[,]
                    {
                        { 1, 2 },
                        { 3, 4 }
                    };
                }
            }
        }
    }
}
