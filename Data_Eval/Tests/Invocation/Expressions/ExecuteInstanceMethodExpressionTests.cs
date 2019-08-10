using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Data.Eval.Invocation.Expressions;

namespace Tests.Invocation.Expressions
{
	[TestFixture]
	public class ExecuteInstanceMethodExpressionTests
	{
		[Test]
		public void ExecuteInstanceMethodExpression_NoArgumentsReturnInt()
		{
			var example = new ExampleClass();

			var func = new ExecuteInstanceMethodExpression()
				.GetFunc(
					typeof(ExampleClass),
					"GetIntValue");

			int testValue = (int)func(
				example, 
				new object[] { });

			Assert.AreEqual(3, testValue);
		}

		public class ExampleClass
		{
			public int GetIntValue()
			{
				return 3;
			}
		}
	}
}
