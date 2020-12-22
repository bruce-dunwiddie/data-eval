using System;
using System.Linq;

namespace Data.Eval.CodeWriting
{
	internal sealed class CSharpClassNameFormatter
	{
		public string GetFullName(
			Type type)
		{
			if (type == typeof(System.Dynamic.ExpandoObject))
			{
				return "dynamic";
			}
			else if (type.IsNotPublic)
			{
				// going to have to wrap the class with an accessor class

				// anonymous type comes in for example as <>f__AnonymousType0`1
				// with no namespace.
				// will become __f__AnonymousType0_1

				string name = "";

				if (type.Namespace != null)
				{
					name += type.Namespace.Replace(".", "_") + "_";
				}

				name += type.Name
					.Replace("<", "_")
					.Replace(">", "_")
					.Replace("`", "_")
					// inner class
					.Replace("+", ".");

				return name;
			}
			else if (
				type.IsGenericType &&
				type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				// handle nullable types

				// e.g. System.Nullable`1 for System.Int32?

				Type underlyingType = type.GetGenericArguments().Single();

				string name =
					underlyingType.FullName.Replace('+', '.') + '?';

				return name;
			}
			else if (
				type.IsGenericType)
			{
				Type[] genericTypes = type.GetGenericArguments();

				string name = type.Name;

				// trim `1
				if (name.Contains("`"))
				{
					name = name.Substring(0, name.LastIndexOf('`'));
				}

				name = type.Namespace
					+ "."
					+ name.Replace('+', '.')
					+ "<"
					// recursion
					+ String.Join(", ", genericTypes
						.Select(t =>
						{
							if (t.IsGenericParameter)
							{
								return t.Name;
							}
							else
							{
								return GetFullName(t);
							}
						}).ToArray())
					+ ">";

				return name;
			}
			else
			{
				string name = type.FullName.Replace('+', '.');

				return name;
			}
		}
	}
}
