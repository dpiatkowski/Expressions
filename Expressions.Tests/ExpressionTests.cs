using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using Xunit;

namespace Expressions.Test
{
    public abstract class ExpressionTests
    {
        public ExpressionLanguage Language { get; private set; }

        private const char SEPARATOR_CHAR = ';';
        protected delegate void LineProcessor(string[] lineParts);

        protected ExpressionOwner MyValidExpressionsOwner = new ExpressionOwner();
        protected ExpressionContext MyGenericContext;
        protected ExpressionContext MyValidCastsContext;

        protected ExpressionContext MyCurrentContext;

        protected static readonly CultureInfo TestCulture = CultureInfo.GetCultureInfo("en-CA");

        public ExpressionTests(ExpressionLanguage language)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            Language = language;

            MyValidExpressionsOwner = new ExpressionOwner();

            MyGenericContext = this.CreateGenericContext(MyValidExpressionsOwner);

            var imports = new[]
            {
                new Import(typeof(Monitor)),
                new Import("Convert", typeof(Convert)),
                new Import(typeof(Guid)),
                new Import(typeof(Version)),
                new Import(typeof(DayOfWeek)),
                new Import("DayOfWeek", typeof(DayOfWeek)),
                new Import(typeof(ValueType)),
                new Import(typeof(IComparable)),
                new Import(typeof(ICloneable)),
                new Import(typeof(Array)),
                new Import(typeof(System.Delegate)),
                //new Import(typeof(AppDomainInitializer)), // TODO: nope
                new Import(typeof(System.Text.Encoding)),
                new Import(typeof(System.Text.ASCIIEncoding)),
                new Import(typeof(ArgumentException))
           };

            ExpressionContext context = new ExpressionContext(imports, MyValidExpressionsOwner);

            //context.Options.OwnerMemberAccess = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

            MyValidCastsContext = context;

            // For testing virtual properties
            //TypeDescriptor.AddProvider(new UselessTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(int))), typeof(int));
            //TypeDescriptor.AddProvider(new UselessTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(string))), typeof(string));

            Initialize();
        }


        protected virtual void Initialize()
        {
        }

        protected ExpressionContext CreateGenericContext(object owner)
        {
            var imports = new[]
            {
                new Import("Mouse", typeof(Mouse)),
                new Import("Monitor", typeof(Monitor)),
                new Import("Math", typeof(Math)),
                new Import("Uri", typeof(Uri)),
                new Import("DateTime", typeof(DateTime)),
                new Import("Convert", typeof(Convert)),
                new Import("Type", typeof(Type)),
                new Import("DayOfWeek", typeof(DayOfWeek)),
                new Import("ConsoleModifiers", typeof(ConsoleModifiers)),
                new Import("ns1", new Import("ns2", typeof(Math)))
            };

            var context = new ExpressionContext(imports, owner);

            //context.Options.OwnerMemberAccess = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

            context.Variables.Add("varInt32", 100);
            context.Variables.Add("varDecimal", new decimal(100));
            context.Variables.Add("varString", "string");

            return context;
        }

        protected void AssertCompileException(string expression)
        {
            try
            {
                new DynamicExpression(expression, Language);
                Assert.True(false);
            }
            catch
            {
            }
        }

        protected void AssertCompileException(string expression, ExpressionContext context)
        {
            AssertCompileException(expression, context, null);
        }

        protected void AssertCompileException(string expression, ExpressionContext context, BoundExpressionOptions options)
        {
            try
            {
                new DynamicExpression(expression, Language).Bind(context, options);
                Assert.True(false, "Compile exception expected");
            }
            catch
            {
            }
        }

        protected void AssertCompileException(string expression, ExpressionContext context, BoundExpressionOptions options, ExpressionsExceptionType exceptionType)
        {
            try
            {
                new DynamicExpression(expression, Language).Bind(context, options);
                Assert.True(false, "Compile exception expected");
            }
            catch (ExpressionsException ex)
            {
                Assert.Equal(exceptionType, ex.Type);
            }
        }

        protected void DoTest(DynamicExpression expression, ExpressionContext expressionContext, string result, Type resultType, CultureInfo testCulture)
        {
            if (ReferenceEquals(resultType, typeof(object)))
            {
                Type expectedType = Type.GetType(result, false, true);

                if (expectedType == null)
                {
                    // Try to get the type from the Tests assembly
                    result = string.Format("{0}.{1}", typeof(ExpressionTests).Namespace, result);
                    expectedType = this.GetType().Assembly.GetType(result, true, true);
                }

                object expressionResult = expression.Invoke(expressionContext, new BoundExpressionOptions
                {
                    AllowPrivateAccess = true,
                    ResultType = resultType
                });

                if (object.ReferenceEquals(expectedType, typeof(void)))
                {
                    Assert.NotNull(expressionResult);
                }
                else
                {
                    Assert.IsType(expectedType, expressionResult);
                }

            }
            else
            {
                TypeConverter tc = TypeDescriptor.GetConverter(resultType);

                object expectedResult = tc.ConvertFromString(null, CultureInfo.CurrentCulture, result);
                object actualResult = expression.Invoke(expressionContext, new BoundExpressionOptions
                {
                    AllowPrivateAccess = true,
                    ResultType = resultType
                });

                expectedResult = RoundIfReal(expectedResult);
                actualResult = RoundIfReal(actualResult);

                Assert.Equal(expectedResult, actualResult);
            }
        }

        protected object RoundIfReal(object value)
        {
            if (object.ReferenceEquals(value.GetType(), typeof(double)))
            {
                double d = (double)value;
                d = Math.Round(d, 4);
                return d;
            }
            else if (object.ReferenceEquals(value.GetType(), typeof(float)))
            {
                float s = (float)value;
                s = (float)Math.Round(s, 4);
                return s;
            }
            else
            {
                return value;
            }
        }

        protected void ProcessScriptTests(string scriptFileName, LineProcessor processor)
        {
            using (var file = File.OpenRead(GetTestDataFilePath(scriptFileName)))
            using (var sr = new StreamReader(file))
            {
                ProcessLines(sr, processor);
            }
        }

        private void ProcessLines(TextReader sr, LineProcessor processor)
        {
            int lineNumber = 1;

            while (sr.Peek() != -1)
            {
                var line = sr.ReadLine();

                ProcessLine(line, processor, lineNumber);

                lineNumber++;
            }
        }

        private void ProcessLine(string line, LineProcessor processor, int lineNumber)
        {
            if (line.StartsWith("'"))
            {
                return;
            }

            try
            {
                string[] arr = line.Split(SEPARATOR_CHAR);
                processor(arr);
            }
            catch (Exception ex)
            {
                Assert.True(false, $"[{lineNumber}]: {line} -> {ex}");
            }
        }

        protected string GetIndividualTest(string testName)
        {
            using (Stream s = File.OpenRead(GetTestDataFilePath("IndividualTests.xml")))
            {
                var document = new XmlDocument();

                document.Load(s);

                foreach (XmlElement element in document.DocumentElement.ChildNodes)
                {
                    if (element.Attributes["Name"].Value == testName)
                    {
                        return element.InnerText;
                    }
                }
            }

            throw new ArgumentException("Could not find test");
        }

        private string GetTestDataFilePath(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return Path.Combine(
                Path.GetDirectoryName(assembly.Location),
                Language == ExpressionLanguage.Csharp ? "CsharpLanguage" : "FleeLanguage",
                "BulkTests",
                "TestScripts",
                fileName
            );
        }
    }
}
