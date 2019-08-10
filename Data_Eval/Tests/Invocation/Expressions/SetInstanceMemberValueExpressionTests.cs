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
	public class SetInstanceMemberValueExpressionTests
	{
		[Test]
		public void SetInstanceMemberValueExpression_Int()
		{
			var example = new ExampleClass
			{
				IntValue = 2
			};

			var action = new SetInstanceMemberValueExpression()
				.GetAction(
					typeof(ExampleClass),
					"IntValue");

			action(
				example,
				3);

			Assert.AreEqual(
				3, 
				example.IntValue);
		}

		[Test]
		public void SetInstanceMemberValueExpression_PrivateInt()
		{
			var example = new ExampleClass();

			var action = new SetInstanceMemberValueExpression()
				.GetAction(
					typeof(ExampleClass),
					"PrivateIntValue");

			action(
				example,
				3);

			Assert.AreEqual(
				3, 
				example.GetPrivateIntValue());
		}

		public class ExampleClass
		{
			public int IntValue;
			private int PrivateIntValue;

			public int GetPrivateIntValue()
			{
				return PrivateIntValue;
			}
		}
	}
}
