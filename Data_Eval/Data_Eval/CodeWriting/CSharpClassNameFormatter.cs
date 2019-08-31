using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
			else if (
				type.IsGenericType &&
				type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
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
