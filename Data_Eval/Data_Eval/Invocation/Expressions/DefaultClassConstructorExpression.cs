using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

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

		// http://stackoverflow.com/questions/390578/creating-instance-of-type-without-default-constructor-in-c-sharp-using-reflectio/16162475#16162475

		//public static class New<T>
		//{
		//	public static readonly Func<T> Instance = Creator();

		//	static Func<T> Creator()
		//	{
		//		Type t = typeof(T);
		//		if (t == typeof(string))
		//			return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();

		//		if (t.HasDefaultConstructor())
		//			return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();

		//		return () => (T)FormatterServices.GetUninitializedObject(t);
		//	}
		//}

		//public static bool HasDefaultConstructor(this Type t)
		//{
		//	return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
		//}
	}
}
