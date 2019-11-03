using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualBasic;
using Xunit;

namespace Expressions.Test.FleeLanguage
{
    public class IndividualTests : Test.ExpressionTests
    {
        public IndividualTests()
            : base(ExpressionLanguage.Flee)
        {
        }

        [Fact(DisplayName = "Test that we properly handle newline escapes in strings")]
        public void TestStringNewlineEscape()
        {
            var e = new DynamicExpression<string>("\"a\\r\\nb\"", ExpressionLanguage.Flee);
            string s = e.Invoke();
            string expected = string.Format("a{0}b", ControlChars.CrLf);
            Assert.Equal(expected, s);
        }

        [Fact(DisplayName = "Test that we can parse from multiple threads")]
        public void TestMultiTreadedParse()
        {
            Thread t1 = new Thread(ThreadRunParse);
            t1.Name = "Thread1";

            Thread t2 = new Thread(ThreadRunParse);
            t2.Name = "Thread2";

            ExpressionContext context = new ExpressionContext();

            t1.Start(context);
            t2.Start(context);

            t1.Join();
            t2.Join();
        }

        private void ThreadRunParse(object o)
        {
            ExpressionContext context = (ExpressionContext)o;
            // Test parse
            for (int i = 0; i <= 40 - 1; i++)
            {
                var e = new DynamicExpression("1+1*200", ExpressionLanguage.Flee);
            }
        }

        [Fact(DisplayName = "Test that we can parse from multiple threads")]
        public void TestMultiTreadedEvaluate()
        {
            System.Threading.Thread t1 = new System.Threading.Thread(ThreadRunEvaluate);
            t1.Name = "Thread1";

            System.Threading.Thread t2 = new System.Threading.Thread(ThreadRunEvaluate);
            t2.Name = "Thread2";

            var e = new DynamicExpression("1+1*200", ExpressionLanguage.Flee);

            t1.Start(e);
            t2.Start(e);

            t1.Join();
            t2.Join();
        }

        private void ThreadRunEvaluate(object o)
        {
            // Test evaluate
            var e2 = (DynamicExpression)o;

            for (int i = 0; i <= 40 - 1; i++)
            {
                int result = (int)e2.Invoke();
                Assert.Equal(1 + 1 * 200, result);
            }
        }

        [Fact(DisplayName = "Test evaluation of generic expressions")]
        public void TestGenericEvaluate()
        {
            ExpressionContext context = default(ExpressionContext);
            context = new ExpressionContext();

            var e1 = new DynamicExpression<int>("1000", ExpressionLanguage.Flee);
            Assert.Equal(1000, e1.Invoke(context));

            var e2 = new DynamicExpression<double>("1000.25", ExpressionLanguage.Flee);
            Assert.Equal(1000.25, e2.Invoke(context));

            var e3 = new DynamicExpression<double>("1000", ExpressionLanguage.Flee);
            Assert.Equal(1000.0, e3.Invoke(context));

            var e4 = new DynamicExpression<ValueType>("1000", ExpressionLanguage.Flee);
            ValueType vt = e4.Invoke(context);
            Assert.Equal(1000, vt);

            var e5 = new DynamicExpression<object>("1000 + 2.5", ExpressionLanguage.Flee);
            object o = e5.Invoke(context);
            Assert.Equal(1000 + 2.5, o);
        }

        [Fact(DisplayName = "Test expression imports")]
        public void TestImports()
        {
            ExpressionContext context;

            context = new ExpressionContext();
            // Import math type directly
            context.Imports.Add(new Import(typeof(Math)));

            // Should be able to see PI without qualification
            var e = new DynamicExpression<double>("pi", ExpressionLanguage.Flee);
            Assert.Equal(Math.PI, e.Invoke(context));

            context = new ExpressionContext(null, MyValidExpressionsOwner);
            // Import math type with prefix
            context.Imports.Add(new Import("math", typeof(Math)));

            // Should be able to see pi by qualifying with Math	
            e = new DynamicExpression<double>("math.pi", ExpressionLanguage.Flee);
            Assert.Equal(Math.PI, e.Invoke(context));

            // Import nothing
            context = new ExpressionContext();
            // Should not be able to see PI
            AssertCompileException("pi");
            AssertCompileException("math.pi");

            // Test importing of builtin types
            new DynamicExpression<double>("double.maxvalue", ExpressionLanguage.Flee).Bind(null);
            new DynamicExpression<string>("string.concat(\"a\", \"b\")", ExpressionLanguage.Flee).Bind(null);
            new DynamicExpression<long>("long.maxvalue * 2", ExpressionLanguage.Flee).Bind(null);
        }

        [Fact(DisplayName = "Test expressions value type variables")]
        public void TestValueTypeVariables()
        {
            var context = new ExpressionContext();
            var variables = context.Variables;

            variables.Add("a", 100);
            variables.Add("b", -100);
            variables.Add("c", DateTime.Now);

            var e1 = new DynamicExpression<int>("a+b", ExpressionLanguage.Flee);
            var result = e1.Invoke(context);
            Assert.Equal(100 + -100, result);

            variables["B"].Value = 1000;
            result = e1.Invoke(context);
            Assert.Equal(100 + 1000, result);

            var e2 = new DynamicExpression<string>("c.tolongdatestring() + c.Year.tostring()", ExpressionLanguage.Flee);
            Assert.Equal(DateTime.Now.ToLongDateString() + DateTime.Now.Year, e2.Invoke(context));
        }

        [Fact(DisplayName = "Test expressions reference type variables")]
        public void TestReferenceTypeVariables()
        {
            var context = new ExpressionContext();
            var variables = context.Variables;

            variables.Add("a", "string");
            variables.Add("b", "100");

            var e = new DynamicExpression<string>("a + b + a.tostring()", ExpressionLanguage.Flee);
            var result = e.Invoke(context);
            Assert.Equal("string" + 100 + "string", result);

            variables["a"].Value = "test";
            variables["b"].Value = "1";
            result = e.Invoke(context);
            Assert.Equal("test" + 1 + "test", result);

            // Test null value
            variables.Add("nullvar", null);

            var e2 = new DynamicExpression<bool>("nullvar = null", ExpressionLanguage.Flee);
            Assert.True(e2.Invoke(context));
        }

        [Fact(DisplayName = "Test that we properly enforce member access on the expression owner")]
        public void TestMemberAccess()
        {
            AccessTestExpressionOwner owner = new AccessTestExpressionOwner();
            ExpressionContext context = new ExpressionContext(null, owner);

            // Private field, access should be denied
            AssertCompileException("privateField1", context, null);

            // Private field but with override allowing access
            var e = new DynamicExpression("privateField2", ExpressionLanguage.Flee);

            // Private field but with override denying access
            AssertCompileException("privateField3", context, new BoundExpressionOptions
            {
                AllowPrivateAccess = true
            });

            // Public field, access should be denied
            AssertCompileException("PublicField1", context, null);

            // Public field, access should be allowed
            e = new DynamicExpression("publicField1", ExpressionLanguage.Flee);
        }

        [Fact(DisplayName = "Test that we can handle long logical expressions and that we properly adjust for long branches")]
        public void TestLongBranchLogical1()
        {
            string expressionText = GetIndividualTest("LongBranch1");
            ExpressionContext context = new ExpressionContext();

            VariableCollection vc = context.Variables;

            vc.Add("M0100_ASSMT_REASON", "0");
            vc.Add("M0220_PRIOR_NOCHG_14D", "1");
            vc.Add("M0220_PRIOR_UNKNOWN", "1");
            vc.Add("M0220_PRIOR_UR_INCON", "1");
            vc.Add("M0220_PRIOR_CATH", "1");
            vc.Add("M0220_PRIOR_INTRACT_PAIN", "1");
            vc.Add("M0220_PRIOR_IMPR_DECSN", "1");
            vc.Add("M0220_PRIOR_DISRUPTIVE", "1");
            vc.Add("M0220_PRIOR_MEM_LOSS", "1");
            vc.Add("M0220_PRIOR_NONE", "1");

            vc.Add("M0220_PRIOR_UR_INCON_bool", true);
            vc.Add("M0220_PRIOR_CATH_bool", true);
            vc.Add("M0220_PRIOR_INTRACT_PAIN_bool", true);
            vc.Add("M0220_PRIOR_IMPR_DECSN_bool", true);
            vc.Add("M0220_PRIOR_DISRUPTIVE_bool", true);
            vc.Add("M0220_PRIOR_MEM_LOSS_bool", true);
            vc.Add("M0220_PRIOR_NONE_bool", true);
            vc.Add("M0220_PRIOR_NOCHG_14D_bool", true);
            vc.Add("M0220_PRIOR_UNKNOWN_bool", true);

            var e = new DynamicExpression(expressionText, ExpressionLanguage.Flee);
            // We only care that the expression is valid and can be evaluated
            object result = e.Invoke(context);
        }

        [Fact(DisplayName = "Test that we can handle long logical expressions and that we properly adjust for long branches")]
        public void TestLongBranchLogical2()
        {
            var expressionText = GetIndividualTest("LongBranch2");

            var expr = new DynamicExpression<bool>(expressionText, ExpressionLanguage.Flee);

            var context = new ExpressionContext();

            Assert.False(expr.Invoke(context));
        }

        [Fact(DisplayName = "Test we can handle an expression owner that is a value type")]
        public void TestValueTypeOwner()
        {
            TestStruct owner = new TestStruct(100);
            ExpressionContext context = this.CreateGenericContext(owner);
            var options = new BoundExpressionOptions
            {
                AllowPrivateAccess = true
            };

            var e = new DynamicExpression<int>("mya.compareto(100)", ExpressionLanguage.Flee);
            int result = e.Invoke(context, options);
            Assert.Equal(0, result);

            e = new DynamicExpression<int>("myA", ExpressionLanguage.Flee);
            result = e.Invoke(context, options);
            Assert.Equal(100, result);

            e = new DynamicExpression<int>("DoStuff()", ExpressionLanguage.Flee);
            result = e.Invoke(context);
            Assert.Equal(100, result);

            DateTime dt = DateTime.Now;
            context = this.CreateGenericContext(dt);

            e = new DynamicExpression<int>("Month", ExpressionLanguage.Flee);
            result = e.Invoke(context);
            Assert.Equal(dt.Month, result);

            var e2 = new DynamicExpression<string>("tolongdatestring()", ExpressionLanguage.Flee);
            Assert.Equal(dt.ToLongDateString(), e2.Invoke(context));
        }

        [Fact(DisplayName = "We should be able to import non-public types if they are in the same module as our owner")]
        public void TestNonPublicImports()
        {
            // ...until we set an owner that is in the same module
            var context = new ExpressionContext(null, new OverloadTestExpressionOwner());
            context.Imports.Add(new Import(typeof(TestImport)));

            var e = new DynamicExpression("DoStuff()", ExpressionLanguage.Flee);
            Assert.Equal(100, (int)e.Invoke(context));
        }

        [Fact(DisplayName = "Test import with nested types")]
        public void TestNestedTypeImport()
        {
            ExpressionContext context = new ExpressionContext();

            // Should be able to import public nested type
            context.Imports.Add(new Import(typeof(NestedA.NestedPublicB)));
            var e = new DynamicExpression("DoStuff()", ExpressionLanguage.Flee);
            Assert.Equal(100, (int)e.Invoke(context));

            // Should be able to import nested internal type now
            context = new ExpressionContext(null, new OverloadTestExpressionOwner());
            context.Imports.Add(new Import(typeof(NestedA.NestedInternalB)));
            e = new DynamicExpression("DoStuff()", ExpressionLanguage.Flee);
            Assert.Equal(100, (int)e.Invoke(context));
        }

        [Fact(DisplayName = "We should not allow access to the non-public members of a variable")]
        public void TestNonPublicVariableMemberAccess()
        {
            var context = new ExpressionContext();
            context.Variables.Add("a", "abc");

            Assert.Throws<ExpressionsException>(() =>
            {
                new DynamicExpression("a.m_length", ExpressionLanguage.Flee).Bind(context);
            });
        }

        [Fact(DisplayName = "We should not compile an expression that accesses a non-public field with the same name as a variable")]
        public void TestFieldWithSameNameAsVariable()
        {
            ExpressionContext context = new ExpressionContext(null, new Monitor());
            context.Variables.Add("doubleA", new ExpressionOwner());
            this.AssertCompileException("doubleA.doubleA", context, null);

            // But it should work for a public member
            context = new ExpressionContext();
            Monitor m = new Monitor();
            context.Variables.Add("I", m);

            var e = new DynamicExpression("i.i", ExpressionLanguage.Flee);
            Assert.Equal(m.I, (int)e.Invoke(context));
        }

        [Fact(DisplayName = "Test handling of on-demand variables")]
        public void TestOnDemandVariables()
        {
            var e = new DynamicExpression("a + b", ExpressionLanguage.Flee);
            Assert.Equal(100 + 100, (int)e.Invoke(new TestExpressionContext()));
        }

        [Fact(DisplayName = "Test that we properly resolve method overloads")]
        public void TestOverloadResolution()
        {
            var owner = new OverloadTestExpressionOwner();
            var context = new ExpressionContext(null, owner);

            // Test value types
            this.DoTestOverloadResolution("valuetype1(100)", context, 1);
            this.DoTestOverloadResolution("valuetype1(100.0f)", context, 2);
            this.DoTestOverloadResolution("valuetype1(100.0)", context, 3);
            this.DoTestOverloadResolution("valuetype2(100)", context, -1);

            // Test value type -> reference type
            this.DoTestOverloadResolution("Value_ReferenceType1(100)", context, 1);
            this.DoTestOverloadResolution("Value_ReferenceType2(100)", context, -1);
            //	with interfaces
            this.DoTestOverloadResolution("Value_ReferenceType3(100)", context, -1);

            // Test reference types
            this.DoTestOverloadResolution("ReferenceType1(\"abc\")", context, 2);
            this.DoTestOverloadResolution("ReferenceType1(b)", context, 1);
            this.DoTestOverloadResolution("ReferenceType2(a)", context, 2);
            //	with interfaces
            this.DoTestOverloadResolution("ReferenceType3(\"abc\")", context, -1);

            // Test nulls
            this.DoTestOverloadResolution("ReferenceType1(null)", context, -1);
            this.DoTestOverloadResolution("ReferenceType2(null)", context, -1);

            // Test ambiguous match
            this.DoTestOverloadResolution("valuetype3(100)", context, -1);
            this.DoTestOverloadResolution("Value_ReferenceType4(100)", context, -1);
            this.DoTestOverloadResolution("ReferenceType4(\"abc\")", context, -1);
            this.DoTestOverloadResolution("ReferenceType4(null)", context, -1);

            // Test access control
            this.DoTestOverloadResolution("Access1(\"abc\")", context, -1);
            this.DoTestOverloadResolution("Access2(\"abc\")", context, -1);
            this.DoTestOverloadResolution("Access2(null)", context, -1);

            // Test with multiple arguments
            this.DoTestOverloadResolution("Multiple1(1.0f, 2.0)", context, 1);
            this.DoTestOverloadResolution("Multiple1(100, 2.0)", context, 2);
            this.DoTestOverloadResolution("Multiple1(100, 2.0f)", context, -1);
        }

        private void DoTestOverloadResolution(string expression, ExpressionContext context, int expectedResult)
        {
            try
            {
                var e = new DynamicExpression<int>(expression, ExpressionLanguage.Flee);
                int result = e.Invoke(context);
                Assert.Equal(expectedResult, result);
            }
            catch (Exception)
            {
                Assert.Equal(-1, expectedResult);
            }
        }

        [Fact(DisplayName = "Test variables that are expressions")]
        public void TestExpressionVariables()
        {
            ExpressionContext context1 = new ExpressionContext();
            context1.Imports.Add(new Import(typeof(Math)));
            var exp1 = new DynamicExpression("sin(pi)", ExpressionLanguage.Flee);
            var boundExp1 = exp1.Bind(context1);

            ExpressionContext context2 = new ExpressionContext();
            context2.Imports.Add(new Import(typeof(Math)));
            var exp2 = new DynamicExpression<double>("cos(pi/2)", ExpressionLanguage.Flee);
            var boundExp2 = exp2.Bind(context2);

            ExpressionContext context3 = new ExpressionContext();
            context3.Variables.Add("a", boundExp1);
            context3.Variables.Add("b", boundExp2);
            var exp3 = new DynamicExpression("cast(a, double) + b", ExpressionLanguage.Flee);

            double a = Math.Sin(Math.PI);
            double b = Math.Cos(Math.PI / 2);

            Assert.Equal(a + b, exp3.Invoke(context3));

            ExpressionContext context4 = new ExpressionContext();
            context4.Variables.Add("a", boundExp1);
            context4.Variables.Add("b", boundExp2);
            var exp4 = new DynamicExpression<double>("(cast(a, double) * b) + (b - cast(a, double))", ExpressionLanguage.Flee);

            Assert.Equal((a * b) + (b - a), exp4.Invoke(context4));
        }

        private class TestExpressionContext : IExpressionContext
        {
            public Type OwnerType { get { return null; } }

            public IList<Import> Imports { get { return null; } }

            public Type GetVariableType(string variable, bool ignoreCase)
            {
                return typeof(int);
            }

            public object Owner { get { return null; } }

            public object GetVariableValue(string variable, bool ignoreCase)
            {
                return 100;
            }
        }
    }
}
