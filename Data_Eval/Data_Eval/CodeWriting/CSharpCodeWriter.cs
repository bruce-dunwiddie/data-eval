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
			StringBuilder classText = new StringBuilder();

			classText.Append("using System;\r\n\r\n");

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

			classText.Append("\tpublic object Eval(){\r\n");

			classText.AppendFormat(
				"\t\t{0};\r\n",
				expression);

			classText.Append("\t}\r\n}\r\n");

			return classText.ToString();
		}

		public string GetClassTextWithNoReturn(
			string expression,
			List<Variable> variables,
			List<string> usings,
			List<string> methods)
		{
			StringBuilder classText = new StringBuilder();

			classText.Append("using System;\r\n\r\n");

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

			classText.Append("\tpublic void Exec(){\r\n");

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
