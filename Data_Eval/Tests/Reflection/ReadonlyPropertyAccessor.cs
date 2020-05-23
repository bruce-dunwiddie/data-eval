using System;
using System.Collections.Generic;
using System.Linq;

using Data.Eval.Reflection;

namespace Tests.Reflection
{
	internal sealed class ReadonlyPropertyAccessor
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
}
