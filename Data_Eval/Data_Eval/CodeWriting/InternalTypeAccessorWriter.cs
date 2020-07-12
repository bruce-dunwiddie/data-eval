using System;
using System.Collections.Generic;
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

			foreach (KeyValuePair<string, Type> property in properties)
			{
				propertiesText.Append(
	$@"public {formatter.GetFullName(property.Value)} {property.Key}
	{{
		get
		{{
			return ({formatter.GetFullName(property.Value)})GetValue(""{property.Key}"");
		}}
	}}");
			}

			string classText =
$@"using System;
using System.Linq;

public sealed class {className}
{{
	private static Dictionary<string, Func<object, object>> properties = null;

	private object innerObject = null;

	public {className}(object innerObject)
	{{
		if (properties == null)
		{{
			properties = new ReadonlyPropertyAccessor().GetProperties(innerObject.GetType());
		}}

		this.innerObject = innerObject;
	}}

	{propertiesText}

	private object GetValue(string property)
	{{
		return properties[property](innerObject);
	}}

	{GetDependencyClasses()}
}}
";

			return classText;
		}

		private string GetDependencyClasses()
		{
			return
	@"internal sealed class ReadonlyPropertyAccessor
	{
		public Dictionary<string, Func<object, object>> GetProperties(Type type)
		{
			PropertyFinder finder = new PropertyFinder();

			GetInstancePropertyValueExpression getGenerator = new GetInstancePropertyValueExpression();

			Dictionary<string, Type> properties = finder.GetProperties(type);

			return properties.ToDictionary(
				keyValue => keyValue.Key,
				keyValue => getGenerator.GetFunc(type, keyValue.Key));
		}
	}

	internal sealed class PropertyFinder
	{
		public Dictionary<string, Type> GetProperties(Type type)
		{
			return type.GetProperties(
				BindingFlags.GetProperty |
				BindingFlags.Instance |
				BindingFlags.Public |
				BindingFlags.DeclaredOnly)
				.ToDictionary(p => p.Name, p => p.PropertyType);
		}
	}

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
				Expression.Parameter(typeof(object), ""i"");

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
	}";
		}
	}
}
