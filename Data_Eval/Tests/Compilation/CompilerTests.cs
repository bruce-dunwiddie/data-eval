using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Data.Eval.Compilation;

using Tests.Resources;

namespace Tests.Compilation
{
	[TestFixture]
	public class CompilerTests
	{
		[Test]
		public void Compiler_CSharpSimpleExpression()
		{
			var compiler = new Compiler();

			Type newType = compiler.Compile(
				ResourceReader.CSharpSimpleExpression);

			Assert.IsNotNull(newType);
		}

		[Test]
		public void Compiler_CSharpSimpleVariable()
		{
			var compiler = new Compiler();

			Type newType = compiler.Compile(
				ResourceReader.CSharpSimpleVariable);

			Assert.IsNotNull(newType);
		}

		[Test]
		public void Compiler_CSharpNullableInt()
		{
			var compiler = new Compiler();

			Type newType = compiler.Compile(
				ResourceReader.CSharpNullableInt);

			Assert.IsNotNull(newType);
		}
	}
}
