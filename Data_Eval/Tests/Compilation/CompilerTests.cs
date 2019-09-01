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

		[Test]
		public void Compiler_Exception()
		{
			var compiler = new Compiler();

			string codeToCompile = @"
				using System;

				public sealed class CustomEvaluator{
					public System.Int32? intValue;
					public object Eval(){
						return intValue + 1
					}
				}";

			CompilationException ex = Assert.Throws<CompilationException>(
				delegate
				{
					compiler.Compile(codeToCompile);
				});

			Assert.AreEqual("Class failed to compile.\n\tLine 6: ; expected", ex.Message);
			Assert.AreEqual(codeToCompile, ex.GeneratedClassCode);
		}
	}
}
