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
				ResourceReader.CSharpSimpleExpression,
				"EvalAssembly",
				"CustomEvaluator");

			Assert.IsNotNull(newType);
		}

		[Test]
		public void Compiler_CSharpSimpleVariable()
		{
			var compiler = new Compiler();

			Type newType = compiler.Compile(
				ResourceReader.CSharpSimpleVariable,
				"EvalAssembly",
				"CustomEvaluator");

			Assert.IsNotNull(newType);
		}

		[Test]
		public void Compiler_CSharpNullableInt()
		{
			var compiler = new Compiler();

			Type newType = compiler.Compile(
				ResourceReader.CSharpNullableInt,
				"EvalAssembly",
				"CustomEvaluator");

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
					compiler.Compile(
						codeToCompile,
						"EvalAssembly",
						"CustomEvaluator");
				});

			Assert.AreEqual("Class failed to compile.\n\tError: ; expected\n\tOn Line 6: return intValue + 1", ex.Message);
			Assert.AreEqual(codeToCompile, ex.GeneratedClassCode);
		}
	}
}
