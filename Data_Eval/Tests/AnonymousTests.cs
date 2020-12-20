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
		//[Ignore(reason: "Haven't fully implemented anonymous types yet.")]
		public void Evaluator_SimpleAnonymousType()
		{
			var test = new
			{
				prop = "something"
			};

			var evaluator = new Evaluator(
				"return a.prop");

			evaluator.DebugFileOutputName = "./Anonymous.cs";

			// TODO: handle array of anonymous type

			// TODO: recursively handle inner anonymous type

			evaluator["a"] = test;

			Assert.AreEqual("something", evaluator.Eval<string>());
		}
	}
}
