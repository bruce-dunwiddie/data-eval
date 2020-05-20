using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Tests.Reflection
{
	[TestFixture]
	public class ReflectionTests
	{
		[Test]
		public void Reflection_AnonymousTypeAccessor()
		{
			var test = new
			{
				prop = "something"
			};

			var accessor = new AnonymousTypeAccessor1(test);

			Assert.AreEqual("something", accessor.prop);
		}
	}
}
