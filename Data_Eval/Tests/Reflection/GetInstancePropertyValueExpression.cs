using System;

namespace Tests.Reflection
{
	internal sealed class GetInstancePropertyValueExpression
	{
		public Func<object, object> GetFunc(
			Type instanceType,
			string memberName)
		{
			System.Reflection.PropertyInfo member = instanceType.GetProperty(
				memberName,
				System.Reflection.BindingFlags.Public |
				System.Reflection.BindingFlags.Instance);

			System.Linq.Expressions.ParameterExpression instance = 
				System.Linq.Expressions.Expression.Parameter(typeof(object), "i");

			System.Linq.Expressions.MemberExpression memberExp = 
				System.Linq.Expressions.Expression.Property(
					System.Linq.Expressions.Expression.Convert(instance, member.DeclaringType),
					member);

			System.Linq.Expressions.Expression<Func<object, object>> getter = 
				System.Linq.Expressions.Expression.Lambda<Func<object, object>>(
					System.Linq.Expressions.Expression.Convert(memberExp, typeof(object)),
					instance);

			Func<object, object> func = getter.Compile();

			return func;
		}
	}
}
