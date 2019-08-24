using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Data.Eval.CodeWriting;

using Tests.Properties;

namespace Tests.CodeWriting
{
	[TestFixture]
	public class CSharpCodeWriterTests
	{
		[Test]
		public void CSharpCodeWriter_SimpleExpression()
		{
			var writer = new CSharpCodeWriter();

			var expression = "return 1 + 1";

			var classText = writer.GetClassTextWithReturn(
				expression: expression,
				variables: new List<CSharpCodeWriter.Variable> { },
				usings: new List<string> { },
				methods: new List<string> { });

			Assert.AreEqual(
				// line ending types don't matter.
				// making sure tests work on Windows and *nix platforms.
				Resources.CSharpSimpleExpression.Replace("\r\n", "\n"),
				classText.Replace("\r\n", "\n"));
		}

		[Test]
		public void CSharpCodeWriter_SimpleVariable()
		{
			var writer = new CSharpCodeWriter();

			var expression = "return intValue + 1";

			var classText = writer.GetClassTextWithReturn(
				expression: expression,
				variables: new List<CSharpCodeWriter.Variable>
				{
					new CSharpCodeWriter.Variable
					{
						Name = "intValue",
						Type = typeof(int)
					}
				},
				usings: new List<string> { },
				methods: new List<string> { });

			Assert.AreEqual(
				Resources.CSharpSimpleVariable.Replace("\r\n", "\n"),
				classText.Replace("\r\n", "\n"));
		}
	}
}
