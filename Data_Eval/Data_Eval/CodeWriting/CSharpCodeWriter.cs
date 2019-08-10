using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Eval.CodeWriting
{
	public sealed class CSharpCodeWriter
	{
		public string GetClassText(
			string expression,
			IEnumerable<Variable> variables = null)
		{
			// TODO: try switching method and variables to static?
			StringBuilder classText = new StringBuilder();

			classText.Append("using System;\r\n\r\n");

			classText.Append("public sealed class CustomEvaluator{\r\n");

			if (variables != null)
			{
				CSharpClassNameFormatter formatter = new CSharpClassNameFormatter();

				foreach (Variable variable in variables)
				{
					classText.AppendFormat(
						"\tprivate {0} {1};\r\n",
						formatter.GetFullName(variable.Type),
						variable.Name);
				}
			}

			classText.Append("\tpublic object Eval(){\r\n");

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
