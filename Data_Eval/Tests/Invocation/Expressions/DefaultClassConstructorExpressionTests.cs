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
	public class DefaultClassConstructorExpressionTests
	{
		[Test]
		public void DefaultClassConstructorExpression_NoConstructor()
		{
			var func = new DefaultClassConstructorExpression()
				.GetFunc(
					typeof(ExampleClass));

			ExampleClass testObj = (ExampleClass)func();

			Assert.IsNotNull(testObj);
		}

		public class ExampleClass
		{
			
		}
	}
}
