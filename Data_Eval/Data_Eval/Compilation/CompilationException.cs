using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Data.Eval.Compilation
{
	[Serializable]
	public sealed class CompilationException : Exception
	{
		public string GeneratedClassCode { get; set; }

		public CompilationException()
		{

		}

		public CompilationException(string message)
			: base(message)
		{

		}

		public CompilationException(string message, Exception innerException)
			: base(message, innerException)
		{

		}

		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("GeneratedClassCode", GeneratedClassCode);

			base.GetObjectData(info, context);
		}
	}
}
