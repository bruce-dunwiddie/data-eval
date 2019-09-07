using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Data.Eval.Compilation
{
	/// <summary>
	///		Exception class that is raised if an error is thrown during compilation
	///		step of Evaluator class containing additional information for troubleshooting.
	/// </summary>
	[Serializable]
	public sealed class CompilationException : Exception
	{
		/// <summary>
		///		Class code that was generated from Evaluator and attempted to be
		///		compiled. Useful for troubleshooting compile error by seeing how
		///		Evaluator used specified values when generating code for compilation.
		/// </summary>
		public string GeneratedClassCode { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public CompilationException()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public CompilationException(string message)
			: base(message)
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public CompilationException(string message, Exception innerException)
			: base(message, innerException)
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private CompilationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("GeneratedClassCode", GeneratedClassCode);

			base.GetObjectData(info, context);
		}
	}
}
