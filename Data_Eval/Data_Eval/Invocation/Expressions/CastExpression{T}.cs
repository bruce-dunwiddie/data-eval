using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Eval.Invocation.Expressions
{
	internal sealed class CastExpression<T>
	{
		public Func<object, T> GetFunc()
		{
			Func<object, T> func = (obj) =>
			{
				return (T)Convert.ChangeType(obj, typeof(T));
			};

			return func;
		}
	}
}
