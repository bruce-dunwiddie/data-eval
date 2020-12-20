using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Data.Eval.Reflection;

namespace Data.Eval.CodeWriting
{
	/// <summary>
	///		Generates a class file wrapping what would otherwise be only an internally
	///		accessible class file, e.g. anonymous type.
	/// </summary>
	internal sealed class InternalTypeAccessorWriter
	{
		public string GetClassTest(
			Type type,
			string className)
		{
			PropertyFinder finder = new PropertyFinder();

			Dictionary<string, Type> properties = finder.GetProperties(type);

			StringBuilder propertiesText = new StringBuilder();

			CSharpClassNameFormatter formatter = new CSharpClassNameFormatter();

			List<string> keys = properties.Keys.ToList();

			for (int i = 0; i < keys.Count; i++)
			{
				string propertyName = keys[i];
				Type propertyType = properties[propertyName];

				if (i > 0)
				{
					propertiesText.Append("\r\n\t\r\n\t");
				}

				propertiesText.Append(
$@"public {formatter.GetFullName(propertyType)} {propertyName}
	{{
		get
		{{
			return ({formatter.GetFullName(propertyType)})GetValue(""{propertyName}"");
		}}
	}}");
			}

			// TODO: should wrapper class implement any interfaces or extend from any classes
			// that wrapped class implements?

			string classText =
$@"public sealed class {className}
{{
	private static System.Collections.Generic.Dictionary<string, Func<object, object>> properties = null;
	
	public {className}(object innerObject)
	{{
		if (properties == null)
		{{
			properties = new ReadonlyPropertyAccessor()
				.GetProperties(
					innerObject.GetType());
		}}

		this.InnerObject = innerObject;
	}}

	public object InnerObject {{ get; set; }}
	
	{propertiesText}
	
	private object GetValue(string property)
	{{
		return properties[property](InnerObject);
	}}
}}
";

			return classText;
		}

		public static string GetDependencyClasses()
		{
			return
	@"internal sealed class ReadonlyPropertyAccessor
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
}
";
		}
	}
}
