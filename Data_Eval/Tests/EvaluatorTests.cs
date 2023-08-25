using System;
using System.Collections.Generic;
using System.Dynamic;

using NUnit.Framework;

using Data.Eval;
using Data.Eval.Compilation;
using System.Diagnostics;

namespace Tests
{
	[TestFixture]
	public class EvaluatorTests
	{
		[Test]
		public void Evaluator_SimpleAddition()
		{
			var evaluator = new Evaluator(
				"return 1 + 1");

			int sum = (int)evaluator.Eval();

			Assert.AreEqual(2, sum);
		}

		[Test]
		public void Evaluator_VariableAddition()
		{
			var evaluator = new Evaluator(
				"return intValue + 1");

			evaluator.SetVariable(
				"intValue",
				2);

			int sum = (int)evaluator.Eval();

			Assert.AreEqual(3, sum);
		}

		[Test]
		public void Evaluator_SetVariable()
		{
			var evaluator = new Evaluator(
				"return intValue++");

			evaluator.SetVariable(
				"intValue",
				2);

			int oldValue = (int)evaluator.Eval();

			Assert.AreEqual(2, oldValue);

			int newValue = (int)evaluator.GetVariable(
				"intValue");

			Assert.AreEqual(3, newValue);
		}

		[Test]
		public void Evaluator_ComplexVariable()
		{
			var evaluator = new Evaluator(
				"return values[\"key\"]++");

			Dictionary<string, int> values = new Dictionary<string, int>
			{
				{ "key", 2 }
			};

			evaluator.SetVariable(
				"values",
				values);

			int oldValue = (int)evaluator.Eval();

			Assert.AreEqual(2, oldValue);

			int newValue = values["key"];

			Assert.AreEqual(3, newValue);
		}

		[Test]
		public void Evaluator_SimpleMathQuestion()
		{
			var answer = Evaluator.Eval("var x = 3; var y = 5; return 3 * x + y;");
			Assert.AreEqual(14, answer);
		}

		[Test]
		public void Evaluator_SimpleMathQuestionUsingConstructor()
		{
			var answer = new Evaluator("var x = 3; var y = 5; return 3 * x + y;").Eval();
			Assert.AreEqual(14, answer);
		}

		[Test]
		public void Evaluator_CastReturnType()
		{
			var answer = new Evaluator("return 1").Eval<double>();
			Assert.AreEqual(1, answer);
		}

		[Test]
		public void Evaluator_CastReturnTypeString()
		{
			var answer = new Evaluator("return 1").Eval<string>();
			Assert.AreEqual("1", answer);
		}

		[Test]
		public void Evaluator_ReturnTypeParseString()
		{
			var answer = new Evaluator("return \"1\"").Eval<double>();
			Assert.AreEqual(1, answer);
		}

		[Test]
		public void Evaluator_UpdateVariable()
		{
			var evaluator = new Evaluator(
				"intValue++");

			evaluator["intValue"] = 2;

			evaluator.Exec();

			int newValue = (int)evaluator["intValue"];

			Assert.AreEqual(3, newValue);
		}

		[Test]
		public void Evaluator_EvalAddMethod()
		{
			var eval = new Evaluator("return AddNumbers(x, y)");

			eval["x"] = 2;
			eval["y"] = 3;

			eval.AddMethod(@"
				int AddNumbers(int first, int second)
				{
					return first + second;
				}");

			Assert.AreEqual(5, eval.Eval<int>());
		}

		[Test]
		public void Evaluator_ExecAddMethod()
		{
			var eval = new Evaluator("z = AddNumbers(x, y)");

			eval["x"] = 2;
			eval["y"] = 3;
			eval["z"] = 0;

			eval.AddMethod(@"
				int AddNumbers(int first, int second)
				{
					return first + second;
				}");

			eval.Exec();

			Assert.AreEqual(5, eval["z"]);
		}

		[Test]
		public void Evaluator_DynamicVariable()
		{
			dynamic test = new ExpandoObject();
			test.prop = "something";

			var evaluator = new Evaluator(
				"return a.prop");

			evaluator["a"] = test;

			Assert.AreEqual("something", evaluator.Eval<string>());
		}

		[Test]
		public void Evaluator_NullVariableComparison()
		{
			var evaluator = new Evaluator(
				"return test == null");

			evaluator.SetVariable(
				"test",
				null,
				typeof(string));

			Assert.IsTrue(evaluator.Eval<bool>());
		}

		[Test]
		public void Evaluator_LinqReference()
		{
			var list = new string[]
			{
				"one",
				"two"
			};

			var evaluator = new Evaluator(
				"return list.Where(s => s.StartsWith(\"o\")).Count()");

			evaluator.SetVariable(
				"list",
				list);

			Assert.AreEqual(1, evaluator.Eval<int>());
		}

		[Ignore("Don't want to have an intensive long running test unless needed.")]
		[Test]
		public void Evaluator_LoopedExceptionTime()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			for (int loopCount = 0; loopCount < 200; loopCount++)
			{
				CompilationException ex = Assert.Throws<CompilationException>(
					delegate
					{
						var blah = Evaluator.Eval("return blah");
					});
			}

			stopwatch.Stop();

			Assert.Less(stopwatch.ElapsedMilliseconds, 3000);
		}
	}
}
