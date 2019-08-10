using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

using Microsoft.CSharp;

namespace Data.Eval.Compilation
{
	public sealed class Compiler
	{
		public Type Compile(
			string classText)
		{
			using (CodeDomProvider provider = new CSharpCodeProvider())
			{
				CompilerParameters options = new CompilerParameters
				{
					CompilerOptions = "/t:library /optimize",
					GenerateInMemory = true,
					IncludeDebugInformation = false,
					TreatWarningsAsErrors = false,
					WarningLevel = 3,
					GenerateExecutable = false
				};

				CompilerResults compileResults = provider.CompileAssemblyFromSource(options, classText);

				if (compileResults.Errors.HasErrors)
				{
					string exceptionMessage = "Class failed to compile.";

					foreach (CompilerError error in compileResults.Errors)
					{
						if (!error.IsWarning)
						{
							exceptionMessage += "\n\tLine " + error.Line.ToString() + ": " + error.ErrorText;
						}
					}

					throw new CompilationException(exceptionMessage)
					{
						GeneratedClassCode = classText
					};
				}

				return compileResults.CompiledAssembly.GetType("CustomEvaluator");
			}
		}
	}
}
