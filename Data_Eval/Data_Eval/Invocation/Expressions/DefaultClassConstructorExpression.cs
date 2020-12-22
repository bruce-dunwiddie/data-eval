using System;
using System.Linq.Expressions;

namespace Data.Eval.Invocation.Expressions
{
	internal sealed class DefaultClassConstructorExpression
	{
		public Func<object> GetFunc(
			Type instanceType)
		{
			NewExpression constructorExp = Expression.New(instanceType);

			Expression<Func<object>> constructor = Expression.Lambda<Func<object>>(constructorExp);

			Func<object> func = constructor.Compile();

			return func;
		}
	}
}
