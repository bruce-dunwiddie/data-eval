using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Data.Eval;

namespace Tests
{
	[TestFixture]
	public class EvaluatorTests
	{
		[Test]
		public void Evaluator_SimpleAddition()
		{
			var asm = Assembly.GetExecutingAssembly();
			var pk = Assembly.GetExecutingAssembly().GetName().GetPublicKey();

			string publicKey = "";

			for (int i = 0; i < pk.GetLength(0); i++)
				publicKey += String.Format("{0:x2}", pk[i]);

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
	}
}
