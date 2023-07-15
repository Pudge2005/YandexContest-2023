using System.Text;

namespace DevourDev.SheetCodeUtility.Testing
{
    public sealed class InOutTester
    {
        public sealed class TestResult
        {
            private readonly bool _passed;
            private readonly TimeSpan _elapsed;
            private readonly string _input;
            private readonly string _expectedOutput;
            private readonly string _facticalOutput;


            internal TestResult(bool passed, TimeSpan elapsed, string input,
                string expectedOutput, string facticalOutput)
            {
                _passed = passed;
                _elapsed = elapsed;
                _input = input;
                _expectedOutput = expectedOutput;
                _facticalOutput = facticalOutput;
            }


            public bool Passed => _passed;
            public TimeSpan Elapsed => _elapsed;
            public string Input => _input;
            public string ExpectedOutput => _expectedOutput;
            public string FacticalOutput => _facticalOutput;


            public override string ToString()
            {
                if (_passed)
                {
                    return $"SUCCESS! Elapsed: {Elapsed.TotalMilliseconds} ms";
                }
                else
                {
                    return $"FAILURE! Expected:{Environment.NewLine}{(ExpectedOutput.Length > 128 ? ExpectedOutput.Length.ToString() + "length" : ExpectedOutput)}{Environment.NewLine}" +
                        $"Received:{Environment.NewLine}{(FacticalOutput.Length > 128 ? FacticalOutput.Length.ToString() + "length" : FacticalOutput)}";
                }
            }
        }


        public sealed class TestBuilder
        {
            private readonly InOutTester _tester;
            private readonly StringBuilder _sb = new();

            private string? _input;


            internal TestBuilder(InOutTester tester)
            {
                _tester = tester;
            }


            public TestBuilder BeginExpectedOutput()
            {
                _input = _sb.ToString();
                _sb.Clear();

                return this;
            }

            public TestBuilder Write(string value)
            {
                _sb.Append(value);

                return this;
            }

            public TestBuilder WriteLine(string value)
            {
                _sb.Append(value)
                    .AppendLine();

                return this;
            }

            public TestBuilder AddTest()
            {
                _tester.AddTest(_input!, _sb.ToString());
                _sb.Clear();

                return this;
            }
        }


        private sealed class TestData
        {
            private readonly string _input;
            private readonly string _expectedOutput;


            public TestData(string input, string expectedOutput)
            {
                _input = input;
                _expectedOutput = expectedOutput;
            }


            public string Input => _input;
            public string ExpectedOutput => _expectedOutput;
        }


        private readonly List<TestData> _testDatas = new();


        public void AddTest(string input, string expectedOutput)
        {
            _testDatas.Add(new(PostProcess(input), PostProcess(expectedOutput)));
        }

        public TestBuilder GetTestBuilder()
        {
            return new TestBuilder(this);
        }

        public TestResult[] StartTests(System.Action testMethod, bool stopOnFail = true, bool throwException = false)
        {
            var tests = _testDatas;
            var testsCount = tests.Count;
            TestResult[] results = new TestResult[testsCount];
            var sb = new StringBuilder();
            var tmpIn = Console.In;
            var tmpOut = Console.Out;
            var sw = new System.Diagnostics.Stopwatch();

            try
            {
                for (int i = 0; i < testsCount; i++)
                {
                    var test = tests[i];
                    var reader = new StringReader(test.Input);
                    var writer = new StringWriter(sb);

                    Console.SetIn(reader);
                    Console.SetOut(writer);

                    TestResult result;

                    try
                    {
                        sw.Restart();
                        testMethod?.Invoke();
                        sw.Stop();
                        result = CreateResult(test, sw.Elapsed, sb.ToString());
                    }
                    catch (Exception ex)
                    {
                        result = CreateResultFromException(test, sw.Elapsed, ex);

                        if (throwException)
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        sb.Clear();
                        reader.Dispose();
                        writer.Dispose();
                    }

                    results[i] = result;

                    if (stopOnFail && !result.Passed)
                    {
                        return results[0..(i + 1)];
                    }
                }
            }
            finally
            {
                Console.SetIn(tmpIn);
                Console.SetOut(tmpOut);
            }

            return results;
        }


        private static TestResult CreateResultFromException(TestData test, TimeSpan elapsed, Exception exception)
        {
            return new(false, elapsed, test.Input, test.ExpectedOutput, exception.Message);
        }

        private static TestResult CreateResult(TestData test, TimeSpan elapsed, string output)
        {
            output = PostProcess(output);
            bool passed = test.ExpectedOutput == output;
            return new(passed, elapsed, test.Input, test.ExpectedOutput, passed ? string.Empty : output);
        }

        private static string PostProcess(string output)
        {
            var lines = output.Split(Environment.NewLine, options: StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
            }

            output = string.Join(Environment.NewLine, lines);
            return output;
        }
    }
}
