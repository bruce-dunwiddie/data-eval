using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tests.Resources
{
    public static class ResourceReader
    {
		public static string CSharpNullableInt
		{
			get
			{
				return ReadResourceFile("CSharpNullableInt.txt");
			}
		}

		public static string CSharpSimpleExpression
		{
			get
			{
				return ReadResourceFile("CSharpSimpleExpression.txt");
			}
		}

		public static string CSharpSimpleVariable
		{
			get
			{
				return ReadResourceFile("CSharpSimpleVariable.txt");
			}
		}

		public static string SimpleAnonymousTestWrapper
		{
			get
			{
				return ReadResourceFile("SimpleAnonymousTestWrapper.txt");
			}
		}

		public static string SimpleEvaluator
		{
			get
			{
				return ReadResourceFile("SimpleEvaluator.txt");
			}
		}

		private static string ReadResourceFile(string fileName)
		{
			using (StreamReader reader = new StreamReader("./Resources/" + fileName))
			{
				return reader.ReadToEnd();
			}
		}
	}
}
