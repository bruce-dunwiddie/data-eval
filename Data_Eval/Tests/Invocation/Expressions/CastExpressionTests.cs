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
	public class CastExpressionTests
	{
		[Test]
		public void CastExpression_CastBoxedValue()
		{
            CastExpression<int> exp = new CastExpression<int>();
            Func<object, int> cast = exp.GetFunc();
			object answer = 1.1;

            int castAnswer = cast(answer);

            Assert.AreNotEqual(1, answer);
            Assert.AreEqual(1, castAnswer);
		}
	}
}
