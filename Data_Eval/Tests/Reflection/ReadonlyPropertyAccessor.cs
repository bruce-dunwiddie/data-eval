using System;
using System.Linq;

using Data.Eval.Reflection;

namespace Tests.Reflection
{
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
}
