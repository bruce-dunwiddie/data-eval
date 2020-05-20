using System;

namespace Tests.Reflection
{
	public sealed class AnonymousTypeAccessor1
	{
		private static System.Collections.Generic.Dictionary<string, Func<object, object>> properties = null;

		private object innerObject = null;

		public AnonymousTypeAccessor1(object innerObject)
		{
			if (properties == null)
			{
				properties = new ReadonlyPropertyAccessor().GetProperties(innerObject.GetType());
			}

			this.innerObject = innerObject;
		}

		public string prop
		{
			get
			{
				return (string)GetValue("prop");
			}
		}

		private object GetValue(string property)
		{
			return properties[property](innerObject);
		}
	}
}
