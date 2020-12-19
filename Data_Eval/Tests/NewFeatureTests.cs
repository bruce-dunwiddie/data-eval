using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Data.Eval;

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
	}
}
