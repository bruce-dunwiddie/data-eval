using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Tests.Resources
{
    public static class ResourceReader
    {
		public static string CSharpNullableInt
		{
			get
			{
				using (StreamReader reader = new StreamReader("./Resources/CSharpNullableInt.txt"))
				{
					return reader.ReadToEnd();
				}
			}
		}

		public static string CSharpSimpleExpression
		{
			get
			{
				using (StreamReader reader = new StreamReader("./Resources/CSharpSimpleExpression.txt"))
				{
					return reader.ReadToEnd();
				}
			}
		}

		public static string CSharpSimpleVariable
		{
			get
			{
				using (StreamReader reader = new StreamReader("./Resources/CSharpSimpleVariable.txt"))
				{
					return reader.ReadToEnd();
				}
			}
		}

	}
}
