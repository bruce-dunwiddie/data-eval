using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Eval.CodeWriting
{
	internal sealed class CSharpCodeWriter
	{
		public string GetClassText(
			string expression,
			List<Variable> variables,
			List<string> usings,
			List<string> methods,
			bool withReturn)
		{
			string signature;

			if (withReturn)
			{
				signature = "public object Eval()";
			}
			else
			{
				signature = "public void Eval()";
			}

			return GetClassText(
				expression,
				variables,
				usings,
				methods,
				signature);
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

			classText.Append("public sealed class CustomEvaluator\r\n{\r\n");

			Dictionary<string, string> wrappedClasses = new Dictionary<string, string>();

			if (variables != null && variables.Count > 0)
			{
				CSharpClassNameFormatter formatter = new CSharpClassNameFormatter();

				foreach (Variable variable in variables)
				{
					string variableType = formatter.GetFullName(variable.Type);

					classText.AppendFormat(
						"\tpublic {0} {1};\r\n",
						variableType,
						variable.Name);

					if (variable.Type.IsNotPublic &&
						!wrappedClasses.ContainsKey(variableType))
					{
						wrappedClasses[variableType] = new InternalTypeAccessorWriter().GetClassTest(
							variable.Type,
							variableType);
					}
				}

				classText.Append("\t\r\n");
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
				"\t{0}\r\n\t{{\r\n",
				signature);

			classText.AppendFormat(
				"\t\t{0};\r\n",
				expression);

			classText.Append("\t}\r\n}\r\n");

			if (wrappedClasses.Count > 0)
			{
				classText.Append("\r\n");
				classText.Append(InternalTypeAccessorWriter.GetDependencyClasses());

				foreach (string wrappedClassDefinition in wrappedClasses.Values)
				{
					classText.Append("\r\n");
					classText.Append(wrappedClassDefinition);
				}
			}

			return classText.ToString();
		}

		public struct Variable
		{
			public string Name;

			public Type Type;
		}
	}
}
