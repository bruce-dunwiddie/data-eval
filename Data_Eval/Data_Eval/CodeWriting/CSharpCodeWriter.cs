using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Eval.CodeWriting
{
	internal sealed class CSharpCodeWriter
	{
		public string GetClassTextWithReturn(
			string expression,
			List<Variable> variables,
			List<string> usings,
			List<string> methods)
		{
			return GetClassText(
				expression,
				variables,
				usings,
				methods,
				"public object Eval()");
		}

		public string GetClassTextWithNoReturn(
			string expression,
			List<Variable> variables,
			List<string> usings,
			List<string> methods)
		{
			return GetClassText(
				expression,
				variables,
				usings,
				methods,
				"public void Exec()");
		}

		private string GetClassText(
			string expression,
			List<Variable> variables,
			List<string> usings,
			List<string> methods,
			string signature)
		{
			StringBuilder classText = new StringBuilder();

			classText.Append("using System;\r\n");

			// adding other standard namespaces for convenience
			classText.Append("using System.Linq;\r\n\r\n");

			// TODO: change sealed class to static class?

			if (usings.Count > 0)
			{
				foreach (string usingNamespace in usings)
				{
					classText.AppendFormat(
						"using {0};\r\n",
						usingNamespace);
				}

				classText.Append("\r\n");
			}

			classText.Append("public sealed class CustomEvaluator{\r\n");

			if (variables != null)
			{
				CSharpClassNameFormatter formatter = new CSharpClassNameFormatter();

				foreach (Variable variable in variables)
				{
					classText.AppendFormat(
						"\tpublic {0} {1};\r\n",
						formatter.GetFullName(variable.Type),
						variable.Name);
				}
			}

			if (methods != null)
			{
				foreach (string method in methods)
				{
					classText.AppendFormat(
						"{0}\r\n",
						method);
				}
			}

			classText.AppendFormat(
				"\t{0}{{\r\n",
				signature);

			classText.AppendFormat(
				"\t\t{0};\r\n",
				expression);

			classText.Append("\t}\r\n}\r\n");

			return classText.ToString();
		}

		public struct Variable
		{
			public string Name;

			public Type Type;
		}
	}
}
