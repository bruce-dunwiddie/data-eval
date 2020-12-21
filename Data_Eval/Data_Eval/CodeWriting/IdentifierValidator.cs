using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Eval.CodeWriting
{
	internal static class IdentifierValidator
	{
		private static Dictionary<string, string> keywords = new string[]
			{
				"abstract",
				"as",
				"base",
				"bool",
				"break",
				"byte",
				"case",
				"catch",
				"char",
				"checked",
				"class",
				"const",
				"continue",
				"decimal",
				"default",
				"delegate",
				"do",
				"double",
				"else",
				"enum",
				"event",
				"explicit",
				"extern",
				"false",
				"finally",
				"fixed",
				"float",
				"for",
				"foreach",
				"goto",
				"if",
				"implicit",
				"in",
				"int",
				"interface",
				"internal",
				"is",
				"lock",
				"long",
				"namespace",
				"new",
				"null",
				"object",
				"operator",
				"out",
				"override",
				"params",
				"private",
				"protected",
				"public",
				"readonly",
				"ref",
				"return",
				"sbyte",
				"sealed",
				"short",
				"sizeof",
				"stackalloc",
				"static",
				"string",
				"struct",
				"switch",
				"this",
				"throw",
				"true",
				"try",
				"typeof",
				"uint",
				"ulong",
				"unchecked",
				"unsafe",
				"ushort",
				"using",
				"virtual",
				"void",
				"volatile",
				"while"
		}.ToDictionary(k => k);

		public static bool IsValidIdentifier(string identifier)
		{
			// https://stackoverflow.com/a/45201527

			if (string.IsNullOrEmpty(identifier))
				return false;
			
			if (!char.IsLetter(identifier[0]) && identifier[0] != '_')
				return false;

			if (keywords.ContainsKey(identifier))
				return false;

			for (int ix = 1; ix < identifier.Length; ++ix)
				if (!char.IsLetterOrDigit(identifier[ix]) && identifier[ix] != '_')
					return false;

			return true;
		}
	}
}
