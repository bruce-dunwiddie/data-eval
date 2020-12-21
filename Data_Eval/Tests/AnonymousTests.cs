using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Data.Eval;

namespace Tests
{
	[TestFixture]
	public class AnonymousTests
	{
		[Test]
		public void Evaluator_SimpleAnonymousType()
		{
			var test = new
			{
				prop = "something"
			};

			var evaluator = new Evaluator(
				"return a.prop");

			// TODO: handle array of anonymous type

			// TODO: recursively handle inner anonymous type

			evaluator["a"] = test;

			Assert.AreEqual("something", evaluator.Eval<string>());
		}

		[Test]
		public void Evaluator_MultipleAnonymousTypes()
		{
			var test = new
			{
				prop = "something"
			};

			var intTest = new
			{
				number = 2
			};

			var evaluator = new Evaluator(
				"return a.prop + b.number");

			evaluator["a"] = test;
			evaluator["b"] = intTest;

			Assert.AreEqual("something2", evaluator.Eval<string>());
		}

		[Test]
		[Ignore("Not handling anonymous arrays yet. It would require per element casting from one array to the other.")]
		public void Evaluator_AnonymousArray()
		{
			var test = new []
			{
				new { Make = "Ford", Model = "Explorer"}
			};

			var evaluator = new Evaluator(
				"return a[0].Model");

			evaluator["a"] = test;

			Assert.AreEqual("Explorer", evaluator.Eval<string>());
		}
	}
}
