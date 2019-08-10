using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

using Microsoft.CSharp;

namespace Data.Eval.Compilation
{
	public sealed class Compiler
	{
		public Type Compile(
			string classText)
		{
			string framework = RuntimeInformation.FrameworkDescription;

			// .NET Framework 4.7.3416.0
			// .NET Core 4.6.26328.01
			bool isFullFramework = framework.ToUpper().Contains(".NET FRAMEWORK");

			// try to use CSharpCodeProvider from full framework if possible because of faster compile
			if (isFullFramework)
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
			// default to Roslyn
			else
			{
				SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classText);

				CSharpCompilation compilation = CSharpCompilation.Create(
					"EvalAssembly",
					new[] { syntaxTree },
					new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
					new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

				using (MemoryStream ms = new MemoryStream())
				{
					EmitResult emitResult = compilation.Emit(ms);

					if (!emitResult.Success)
					{
						string exceptionMessage = "Class failed to compile.";

						foreach (var error in emitResult.Diagnostics)
						{							
							if (error.Severity == DiagnosticSeverity.Error || error.IsWarningAsError)
							{								
								exceptionMessage += "\n\tLine " + error.Location.GetLineSpan().StartLinePosition.Line.ToString() + ": " + error.GetMessage();
							}
						}

						throw new CompilationException(exceptionMessage)
						{
							GeneratedClassCode = classText
						};
					}
					else
					{
						Assembly compiledAssembly = Assembly.Load(ms.ToArray());
						return compiledAssembly.GetType("CustomEvaluator");
					}
				}
			}			
		}
	}
}
