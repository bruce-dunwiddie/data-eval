using System;

using NUnit.Framework;

using Data.Eval;

namespace Tests
{
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
	}
}
