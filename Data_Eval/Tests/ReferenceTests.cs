using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Data.Eval;

namespace Tests
{
	[TestFixture]
    public class ReferenceTests
    {
		[Test]
		public void Evaluator_AddCallingAssemblyReference()
		{
			var eval = new Evaluator("return Tests.ReferenceTests.Multiply(x, y)");
			eval["x"] = 2;
			eval["y"] = 3;
			var result = eval.Eval<int>();
			Assert.AreEqual(6, result);
		}

		public static int Multiply(int x, int y)
		{
			return x * y;
		}
    }
}
