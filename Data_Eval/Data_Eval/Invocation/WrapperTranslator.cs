using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Data.Eval.Invocation
{
	internal sealed class WrapperTranslator
	{
		public Action<object, object> GetWrapAndSet(
			Type instanceType,
			string memberName)
		{
			ParameterExpression instance = Expression.Parameter(typeof(object), "i");
			ParameterExpression argument = Expression.Parameter(typeof(object), "a");

			FieldInfo member = instanceType.GetField(
				memberName,
				BindingFlags.Public | BindingFlags.Instance);

			ConstructorInfo constructor = member.FieldType.GetConstructor(
				new Type[]
				{
					typeof(object)
				});

			NewExpression constructorExp = Expression.New(
				constructor,
				argument);

			MemberExpression memberExp = Expression.Field(
				Expression.Convert(instance, instanceType),
				member);

			BinaryExpression assignExp = Expression.Assign(
				memberExp,
				constructorExp);

			Expression<Action<object, object>> setter = Expression.Lambda<Action<object, object>>(
				assignExp,
				instance,
				argument);

			Action<object, object> action = setter.Compile();

			return action;
		}
		
		public Func<object, object> GetGetAndUnwrap(
			Type instanceType,
			string memberName)
		{
			ParameterExpression instance = Expression.Parameter(typeof(object), "i");

			FieldInfo member = instanceType.GetField(
				memberName,
				BindingFlags.Public | BindingFlags.Instance);

			PropertyInfo unwrappedMember = member.FieldType.GetProperty(
				"InnerObject",
				BindingFlags.Public | BindingFlags.Instance);

			MemberExpression memberExp = Expression.Field(
				Expression.Convert(instance, member.DeclaringType),
				member);

			MemberExpression innerExp = Expression.Property(
				memberExp,
				unwrappedMember);

			Expression<Func<object, object>> getter = Expression.Lambda<Func<object, object>>(
				Expression.Convert(innerExp, typeof(object)),
				instance);

			Func<object, object> func = getter.Compile();

			return func;
		}
	}
}
