using System;
using System.Collections.Generic;

namespace Tests.Reflection
{
	/// <summary>
	///		This is a hand created proof of concept class for the code that will need to be
	///		created dynamically to access properties on anonymous types.
	/// </summary>
	public sealed class AnonymousTypeAccessor1
	{
		private static Dictionary<string, Func<object, object>> properties = null;

		public AnonymousTypeAccessor1(object innerObject)
		{
			if (properties == null)
			{
				properties = new ReadonlyPropertyAccessor().GetProperties(innerObject.GetType());
			}

			this.InnerObject = innerObject;
		}

		public object InnerObject { get; set; }

		public string prop
		{
			get
			{
				return (string)GetValue("prop");
			}
		}

		private object GetValue(string property)
		{
			return properties[property](InnerObject);
		}
	}
}
