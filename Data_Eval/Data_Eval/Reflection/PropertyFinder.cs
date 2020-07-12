using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Data.Eval.Reflection
{
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
}
