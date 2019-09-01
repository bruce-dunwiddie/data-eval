using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Data.Eval.Invocation.Expressions
{
	internal sealed class ExecuteInstanceMethodExpression
	{
		public Func<object, object[], object> GetFuncWithReturn(
			Type instanceType,
			string methodName)
		{
			MethodInfo method = instanceType.GetMethod(methodName);

			ParameterExpression instance = Expression.Parameter(typeof(object), "i");

			ParameterExpression allParameters = Expression.Parameter(typeof(object[]), "params");

			Expression methodExp = Expression.Call(
				Expression.Convert(instance, method.DeclaringType),
				method,
				new Expression[] { });

			// http://stackoverflow.com/questions/8974837/expression-of-type-system-datetime-cannot-be-used-for-return-type-system-obje
			if (methodExp.Type.IsValueType)
			{
				methodExp = Expression.Convert(methodExp, typeof(object));
			}

			Expression<Func<object, object[], object>> methodCall = Expression.Lambda<Func<object, object[], object>>(
				methodExp,
				instance,
				allParameters);

			Func<object, object[], object> func = methodCall.Compile();

			return func;
		}

		public Action<object, object[]> GetFuncWithNoReturn(
			Type instanceType,
			string methodName)
		{
			MethodInfo method = instanceType.GetMethod(methodName);

			ParameterExpression instance = Expression.Parameter(typeof(object), "i");

			ParameterExpression allParameters = Expression.Parameter(typeof(object[]), "params");

			Expression methodExp = Expression.Call(
				Expression.Convert(instance, method.DeclaringType),
				method,
				new Expression[] { });

			Expression<Action<object, object[]>> methodCall = Expression.Lambda<Action<object, object[]>>(
				methodExp,
				instance,
				allParameters);

			Action<object, object[]> func = methodCall.Compile();

			return func;
		}
	}
}
