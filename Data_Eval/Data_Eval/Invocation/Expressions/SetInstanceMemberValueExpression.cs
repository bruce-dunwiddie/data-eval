using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Data.Eval.Invocation.Expressions
{
	internal sealed class SetInstanceMemberValueExpression
	{
		public Action<object, object> GetAction(
			Type instanceType,
			string memberName)
		{
			FieldInfo member = instanceType.GetField(
				memberName,
				BindingFlags.Public | BindingFlags.Instance);

			ParameterExpression instance = Expression.Parameter(typeof(object), "i");
			ParameterExpression argument = Expression.Parameter(typeof(object), "a");

			MemberExpression memberExp = Expression.Field(
				Expression.Convert(instance, member.DeclaringType),
				member);

			BinaryExpression assignExp = Expression.Assign(
				memberExp,
				Expression.Convert(argument, member.FieldType));

			Expression<Action<object, object>> setter = Expression.Lambda<Action<object, object>>(
				assignExp,
				instance,
				argument);

			Action<object, object> action = setter.Compile();

			return action;
		}
	}
}
