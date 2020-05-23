using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Data.Eval.Invocation.Expressions
{
	internal sealed class WrapperClassConstructorExpression
	{
		public Func<object, object> GetFunc(
			Type instanceType)
		{
			ConstructorInfo constructor = instanceType.GetConstructor(
				new Type[]
				{
					typeof(object)
				});

			ParameterExpression wrappedObject = Expression.Parameter(
				typeof(object));

			NewExpression constructorExp = Expression.New(
				constructor,
				wrappedObject);

			Expression<Func<object, object>> constructorCall = Expression.Lambda<Func<object, object>>(
				constructorExp,
				wrappedObject);

			Func<object, object> func = constructorCall.Compile();

			return func;
		}
	}
}
