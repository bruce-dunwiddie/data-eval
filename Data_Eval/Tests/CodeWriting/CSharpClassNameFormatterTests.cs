using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Data.Eval.CodeWriting;

namespace Tests.CodeWriting
{
	[TestFixture]
	public class CSharpClassNameFormatterTests
	{
		[Test]
		public void CSharpClassNameFormatter_NullableInt()
		{
			var formatter = new CSharpClassNameFormatter();

			Type testType = typeof(int?);

			var className = formatter.GetFullName(
				testType);

			Assert.AreEqual(
				"System.Int32?",
				className);
		}
	}
}
