using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Data.Eval;
using Data.Eval.Compilation;

using TestExternalReference;

namespace Tests
{
	[TestFixture]
    public class ReferenceTests
    {
		[Test]
		public void Evaluator_EvalAddCallingAssemblyReference()
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

		[Test]
		public void Evaluator_EvalAddUsing()
		{
			var eval = new Evaluator("return ReferenceTests.Multiply(x, y)");
			eval.AddUsing("Tests");
			eval["x"] = 2;
			eval["y"] = 3;
			var result = eval.Eval<int>();
			Assert.AreEqual(6, result);
		}

		[Test]
		public void Evaluator_ExecAddCallingAssemblyReference()
		{
			var eval = new Evaluator("z = Tests.ReferenceTests.Multiply(x, y)");
			eval["x"] = 2;
			eval["y"] = 3;
			eval["z"] = 0;
			eval.Exec();
			Assert.AreEqual(6, eval["z"]);
		}

		[Test]
		public void Evaluator_ExecAddUsing()
		{
			var eval = new Evaluator("message = ExampleClass.HelloWorld");
			eval.AddReference(typeof(TestExternalReference.ExampleClass).Assembly.Location);
			eval.AddUsing("TestExternalReference");
			eval["message"] = "";
			eval.Exec();
			Assert.AreEqual("Hello World", eval["message"]);
		}

		[Test]
		public void Evaluator_ExecAddReferenceRequired()
		{
			// this should fail without the call to AddReference
			var eval = new Evaluator("message = ExampleClass.HelloWorld");
			eval.AddUsing("TestExternalReference");
			eval["message"] = "";

			CompilationException ex = Assert.Throws<CompilationException>(
				delegate
				{
					eval.Exec();
				});

			Assert.IsTrue(
				ex.Message.Contains("The type or namespace name 'TestExternalReference' could not be found (are you missing a using directive or an assembly reference?)"));
		}

		[Test]
		public void Evaluator_ExecAddReferenceFromVariable()
		{
			// this should not fail even without the call to AddReference
			var eval = new Evaluator("return person.Name");

			var person = new ExampleType()
			{
				ID = 1,
				Name = "John"
			};

			eval["person"] = person;

			var name = eval.Eval<string>();

			Assert.AreEqual("John", name);
		}
	}
}
