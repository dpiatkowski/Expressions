using System;
using Expressions.Expressions;
using Xunit;

namespace Expressions.Test.FleeLanguage.ExpressionTests
{
    public class Indexing : TestBase
    {
        [Fact]
        public void SimpleCollection()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "SimpleCollection[0]",
                new MethodCall(
                    new MethodCall(
                        new VariableAccess(typeof(Owner), 0),
                        typeof(Owner).GetMethod("get_SimpleCollection"),
                        null
                    ),
                    typeof(OwnerSimpleCollection).GetMethod("get_Item"),
                    new IExpression[]
                    {
                        new Constant(0)
                    }
                )
            );
        }

        [Fact]
        public void CollectionWithOverload()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "DualCollection[0]",
                new MethodCall(
                    new MethodCall(
                        new VariableAccess(typeof(Owner), 0),
                        typeof(Owner).GetMethod("get_DualCollection"),
                        null
                    ),
                    typeof(OwnerDualCollection).GetMethod("get_Item", new[] { typeof(int) }),
                    new IExpression[]
                    {
                        new Constant(0)
                    }
                )
            );

            Resolve(
                new ExpressionContext(null, new Owner()),
                "DualCollection[\"key\"]",
                new MethodCall(
                    new MethodCall(
                        new VariableAccess(typeof(Owner), 0),
                        typeof(Owner).GetMethod("get_DualCollection"),
                        null
                    ),
                    typeof(OwnerDualCollection).GetMethod("get_Item", new[] { typeof(string) }),
                    new IExpression[]
                    {
                        new Constant("key")
                    }
                )
            );
        }

        [Fact]
        public void UnresolvedIndexWithOverload()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                Resolve(new ExpressionContext(null, new Owner()), "DualCollection[1.7]");
            });
        }

        [Fact]
        public void CannotIndexArrayWithDouble()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                Resolve(new ExpressionContext(null, new Owner()), "SimpleArray[1.7]");
            });

        }

        [Fact]
        public void NoIndexer()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                Resolve(new ExpressionContext(null, new Owner()), "Value[0]");
            });
        }

        [Fact]
        public void FieldIndex()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "SimpleArray[0]",
                new Expressions.Index(
                    new MethodCall(
                        new VariableAccess(typeof(Owner), 0),
                        typeof(Owner).GetMethod("get_SimpleArray"),
                        null
                    ),
                    new Constant(0),
                    typeof(int)
                )
            );
        }

        [Fact]
        public void RankedFieldIndex()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "RankedArray[0,0]",
                new MethodCall(
                    new MethodCall(
                        new VariableAccess(typeof(Owner), 0),
                        typeof(Owner).GetMethod("get_RankedArray"),
                        null
                    ),
                    typeof(int[,]).GetMethod("Get"),
                    new IExpression[]
                    {
                        new Constant(0),
                        new Constant(0)
                    }
                )
            );
        }

        [Fact]
        public void IllegalRank()
        {
            Assert.Throws<ExpressionsException>(() =>
            {
                Resolve(new ExpressionContext(null, new Owner()), "RankedArray[0,0,0]");
            });
        }

        public class Owner
        {
            public int Value { get; set; }

            public OwnerSimpleCollection SimpleCollection { get; set; }

            public OwnerDualCollection DualCollection { get; set; }

            public int[] SimpleArray { get; set; }

            public int[,] RankedArray { get; set; }
        }

        public class OwnerSimpleCollection
        {
            public int this[int key]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }
        }

        public class OwnerDualCollection
        {
            public int this[int key]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string this[string key]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }
        }
    }
}
