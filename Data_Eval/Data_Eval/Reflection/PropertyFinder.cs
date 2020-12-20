using System;
using System.Linq;

namespace Data.Eval.Reflection
{
	internal sealed class PropertyFinder
	{
		/// <summary>
		///		Returns a Dictionary of the public declared instance properties of
		///		a given Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
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
