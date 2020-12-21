using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

using Data.Eval.CodeWriting;

using Tests.Resources;

namespace Tests.CodeWriting
{
	[TestFixture]
	public class AnonymousTypeAccessorWriterTests
	{
		[Test]
		public void AnonymousTypeAccessorWriter_SimpleTest()
		{
			var test = new
			{
				SimpleProperty1 = "simple",
				SimpleNumber = 123
			};

			var writer = new InternalTypeAccessorWriter();

			var classText = writer.GetClassTest(
				test.GetType(), 
				"SimpleAnonymousTestWrapper");

			Assert.AreEqual(
				ResourceReader.SimpleAnonymousTestWrapper.Replace("\r\n", "\n"),
				classText.Replace("\r\n", "\n"));
		}
	}
}
