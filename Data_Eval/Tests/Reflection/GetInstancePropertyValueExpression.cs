using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Tests.Reflection
{
	internal sealed class GetInstancePropertyValueExpression
	{
		public Func<object, object> GetFunc(
			Type instanceType,
			string memberName)
		{
			PropertyInfo member = instanceType.GetProperty(
				memberName,
				BindingFlags.Public |
				BindingFlags.Instance);

			ParameterExpression instance = 
				Expression.Parameter(typeof(object), "i");

			MemberExpression memberExp = 
				Expression.Property(
					Expression.Convert(instance, member.DeclaringType),
					member);

			Expression<Func<object, object>> getter = 
				Expression.Lambda<Func<object, object>>(
					Expression.Convert(memberExp, typeof(object)),
					instance);

			Func<object, object> func = getter.Compile();

			return func;
		}
	}
}
