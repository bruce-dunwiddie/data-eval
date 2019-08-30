using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
			return Compile(
				classText,
				new List<string>());
		}

		public Type Compile(
			string classText,
			List<string> referenceAssemblies)
		{
			// https://stackoverflow.com/questions/23907305/roslyn-has-no-reference-to-system-runtime

			//The location of the .NET assemblies
			var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

			var references = referenceAssemblies
				.Concat(new string[] {

					typeof(object).Assembly.Location,

					/* 
					* Adding some necessary .NET assemblies
					* These assemblies couldn't be loaded correctly via the same construction as above,
					* in specific the System.Runtime.
					*/
					Path.Combine(assemblyPath, "mscorlib.dll"),
					Path.Combine(assemblyPath, "System.dll"),
					Path.Combine(assemblyPath, "System.Core.dll"),
					Path.Combine(assemblyPath, "System.Runtime.dll"),
					Path.Combine(assemblyPath, "System.Linq.Expressions.dll"),
					Path.Combine(assemblyPath, "Microsoft.CSharp.dll"),
					Path.Combine(assemblyPath, "netstandard.dll")
				})
				.Select(r => MetadataReference.CreateFromFile(r))
				.ToArray();

			SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classText);

			CSharpCompilation compilation = CSharpCompilation.Create(
				"EvalAssembly",
				new[] { syntaxTree },
				references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			using (MemoryStream ms = new MemoryStream())
			{
				EmitResult emitResult = compilation.Emit(ms);

				if (!emitResult.Success)
				{
					string exceptionMessage = "Class failed to compile.";

					foreach (var error in emitResult.Diagnostics
						.Where(e => e.Severity == DiagnosticSeverity.Error || e.IsWarningAsError)
						.OrderBy(e => e.Location.GetLineSpan().StartLinePosition.Line))
					{							
						exceptionMessage += "\n\tLine " + error.Location.GetLineSpan().StartLinePosition.Line.ToString() + ": " + error.GetMessage();
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
