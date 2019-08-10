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

namespace Data.Eval.Compilation
{
	internal sealed class Compiler
	{
		public Type Compile(
			string classText)
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
