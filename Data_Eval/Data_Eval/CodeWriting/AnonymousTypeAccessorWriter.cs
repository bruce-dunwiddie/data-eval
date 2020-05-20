using System;
using System.Collections.Generic;
using System.Text;

using Data.Eval.Reflection;

namespace Data.Eval.CodeWriting
{
	internal sealed class AnonymousTypeAccessorWriter
	{
		public string GetClassTest(
			Type type,
			string className)
		{
			PropertyFinder finder = new PropertyFinder();

			Dictionary<string, Type> properties = finder.GetProperties(type);

			StringBuilder classText = new StringBuilder();



			return classText.ToString();
		}

		public string GetDependencyClasses()
		{
			return @"
				internal sealed class ReadonlyPropertyAccessor
				{
					public System.Collections.Generic.Dictionary<string, Func<object, object>> GetProperties(Type type)
					{
						PropertyFinder finder = new PropertyFinder();

						GetInstancePropertyValueExpression getGenerator = new GetInstancePropertyValueExpression();

						System.Collections.Generic.Dictionary<string, Type> properties = finder.GetProperties(type);

						return properties.ToDictionary(
							keyValue => keyValue.Key,
							keyValue => getGenerator.GetFunc(type, keyValue.Key));
					}
				}

				internal sealed class PropertyFinder
				{
					public System.Collections.Generic.Dictionary<string, Type> GetProperties(Type type)
					{
						return type.GetProperties(
							System.Reflection.BindingFlags.GetProperty |
							System.Reflection.BindingFlags.Instance |
							System.Reflection.BindingFlags.Public |
							System.Reflection.BindingFlags.DeclaredOnly)
							.ToDictionary(p => p.Name, p => p.PropertyType);
					}
				}

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
							System.Linq.Expressions.Expression.Parameter(typeof(object), ""i"");

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
				}";
		}
	}
}
