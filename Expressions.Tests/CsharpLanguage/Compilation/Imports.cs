using System;
using Xunit;

namespace Expressions.Test.CsharpLanguage.Compilation
{
    public class Imports : TestBase
    {
        [Fact]
        public void RootImport()
        {
            Resolve(
                new ExpressionContext(new[] { new Import(typeof(Math)) }),
                "Abs(-1)",
                1
            );
        }

        [Fact]
        public void NamespaceImport()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("ns1", typeof(Math)) }),
                "ns1.Abs(-1)",
                1
            );
        }

        [Fact]
        public void NestedNamespaceImport()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("ns1", new Import("ns2", typeof(Math))) }),
                "ns1.ns2.Abs(-1)",
                1
            );
        }

        [Fact]
        public void DoubleNestedNamespaceImport()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("ns1", new Import("ns2", new Import("ns3", typeof(Math)))) }),
                "ns1.ns2.ns3.Abs(-1)",
                1
            );
        }

        [Fact]
        public void NestedImportOnNamespace()
        {
            Resolve(
                new ExpressionContext(
                    new[]
                    {
                        new Import(
                            "ns",
                            new Import(typeof(Math)),
                            new Import(typeof(int))
                        )
                    }
                ),
                "ns.Abs(ns.MinValue + 10)",
                int.MaxValue - 9
            );
        }

        [Fact]
        public void NestedImportOnNestedNamespace()
        {
            Resolve(
                new ExpressionContext(
                    new[]
                    {
                        new Import(
                            "ns1",
                            new Import(typeof(Math)),
                            new Import(
                                "ns2",
                                new Import(typeof(int))
                            )
                        )
                    }
                ),
                "ns1.Abs(ns1.ns2.MinValue + 10)",
                int.MaxValue - 9
            );
        }
    }
}
