using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

using Data.Eval;

using Tests.Resources;

namespace Tests
{
	[TestFixture]
	public class NewFeatureTests
	{
		[Test]
		public void Evaluator_VariableList()
		{
			var evaluator = new Evaluator("return a + b");

			Assert.AreEqual(0, evaluator.VariableNames.Count);

			evaluator["a"] = 1;
			evaluator["b"] = 2;

			Assert.AreEqual(2, evaluator.VariableNames.Count);
			Assert.IsTrue(evaluator.VariableNames.Contains("a"));
			Assert.IsTrue(evaluator.VariableNames.Contains("b"));

			Assert.AreEqual(3, evaluator.Eval());
		}

		[Test]
		public void Evaluator_FailVariableName()
		{
			var evaluator = new Evaluator("return 1");

			Assert.Throws<ArgumentException>(
				delegate { evaluator["1"] = 1; },
				"Invalid value passed in for variable name. " +
					"Valid variable names must start with a letter or underscore, and not contain any whitespace."
				);
		}

		[Test]
		public void Evaluator_SuccessfulVariableNames()
		{
			var evaluator = new Evaluator("return 1");

			Assert.DoesNotThrow(
				delegate { evaluator["a"] = 1; });
			Assert.DoesNotThrow(
				delegate { evaluator["a1"] = 1; });
			Assert.DoesNotThrow(
				delegate { evaluator["_a"] = 1; });
		}

		[Test]
		public void Evaluator_SaveDebugFile()
		{
			var evaluator = new Evaluator("return 1");

			evaluator.DebugFileOutputName = "./Evaluator.cs";

			Assert.AreEqual(1, evaluator.Eval());

			string debugFileContents = File.ReadAllText("./Evaluator.cs");

			Assert.AreEqual(
				ResourceReader.SimpleEvaluator,
				debugFileContents);
		}
	}
}
