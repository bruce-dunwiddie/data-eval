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
	public class GetInstanceMemberValueExpressionTests
	{
		[Test]
		public void GetInstanceMemberValueExpression_Int()
		{
			var example = new ExampleClass
			{
				IntValue = 3
			};

			var func = new GetInstanceMemberValueExpression()
				.GetFunc(
					typeof(ExampleClass),
					"IntValue");

			int testValue = (int)func(example);

			Assert.AreEqual(3, testValue);
		}

		public class ExampleClass
		{
			public int IntValue;
		}
	}
}
