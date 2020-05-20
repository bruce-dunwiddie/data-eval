using System;
using System.Linq;

namespace Data.Eval.Reflection
{
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
}
