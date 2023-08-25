﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Data.Eval.CodeWriting;
using Data.Eval.Compilation;
using Data.Eval.Invocation;
using Data.Eval.Invocation.Expressions;

namespace Data.Eval
{
	/// <summary>
	///		Class for evaluating and executing C# based string expressions
	///		dynamically at runtime.
	/// </summary>
	public sealed class Evaluator
	{
		private readonly string expression;
		private readonly Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
		private readonly List<string> references = new List<string>();
		private readonly List<string> usings = new List<string>();
		private readonly List<string> methods = new List<string>();
		private bool initialized = false;
		private Execution execution = null;
		private bool callerInitialized = false;
		private string caller = null;

		private static Dictionary<string, Execution> compiledTypes = new Dictionary<string, Execution>();

		/// <summary>
		///		Creates an instance of the evaluator by specifying the expression
		///		to be evaluated.
		/// </summary>
		/// <param name="expression">
		///		C# based string expression to be evaluated or executed, e.g.
		///		"return 1+1" for evaluation or
		///		"System.Console.WriteLine(\"Hello World!\")" for execution.
		/// </param>
		public Evaluator(
			string expression)
		{
			this.expression = expression;
		}

		/// <summary>
		///		Sets the value of a variable referenced within the expression prior
		///		to evaluation or returns the value of a variable referenced within the
		///		expression after evaluation.
		/// </summary>
		/// <param name="name">
		///		Name of the variable referenced within the expression.
		/// </param>
		/// <returns>
		///		Value of the variable referenced within the expression after evaluation.
		/// </returns>
		public object this[string name]
		{
			get
			{
				return GetVariable(name);
			}

			set
			{
				SetVariable(
					name,
					value);
			}
		}

		/// <summary>
		///		Returns a list of all the variables that have been added to the
		///		Evaluator context.
		/// </summary>
		public List<string> VariableNames
		{
			get
			{
				return variables.Keys.ToList();
			}
		}

		/// <summary>
		///		If set, the definition of the internal evaluation class will be saved
		///		out to this file upon the first call to Exec or Eval. This file can be
		///		used to troubleshoot syntax errors.
		/// </summary>
		public string DebugFileOutputName { get; set; }

		/// <summary>
		///		Sets the value of a variable referenced within the expression prior
		///		to evaluation.
		/// </summary>
		/// <param name="name">
		///		Name of the variable referenced within the expression.
		/// </param>
		/// <param name="value">
		///		Value of the variable that should be used when evaluating the expression.
		/// </param>
		public void SetVariable(
			string name,
			object value)
		{
			SetVariable(
				name,
				value,
				value.GetType());
		}

		/// <summary>
		///		Sets the value of a variable referenced within the expression prior
		///		to evaluation. This override allows specifying the Type of the variable
		///		instead of trying to introspect it. Also allows for passing null as the
		///		value.
		/// </summary>
		/// <param name="name">
		///		Name of the variable referenced within the expression.
		/// </param>
		/// <param name="value">
		///		Value of the variable that should be used when evaluating the expression.
		/// </param>
		/// <param name="type">
		///		The variable Type.
		/// </param>
		public void SetVariable(
			string name,
			object value,
			Type type)
		{
			if (variables.ContainsKey(name))
			{
				variables[name].Value = value;
			}
			else
			{
				if (IdentifierValidator.IsValidIdentifier(name))
				{
					variables[name] = new Variable
					{
						Type = type,
						Value = value
					};

					initialized = false;
				}
				else
				{
					throw new ArgumentException("Invalid value passed in for variable name. " +
						"Valid variable names must start with a letter or underscore, and not contain any whitespace.");
				}
			}
		}

		/// <summary>
		///		Returns the value of a variable referenced within the
		///		expression after evaluation.
		/// </summary>
		/// <param name="name">
		///		Name of the variable referenced within the expression.
		/// </param>
		/// <returns>
		///		Value of the variable referenced within the expression after evaluation.
		/// </returns>
		public object GetVariable(
			string name)
		{
			return variables[name].Value;
		}

		/// <summary>
		///		Allows methods and types from an external library to be referenced
		///		within an expression by providing the path to the location of the
		///		assembly where they're defined.
		/// </summary>
		/// <param name="assemblyPath">
		///		Absolute or relative path to the location of the referenced
		///		assembly.
		/// </param>
		public void AddReference(
			string assemblyPath)
		{
			references.Add(assemblyPath);

			initialized = false;
		}

		/// <summary>
		///		Allows methods and types from an external library to be referenced
		///		within an expression by providing a reference to the already 
		///		loaded assembly from the calling code.
		/// </summary>
		/// <param name="assembly">
		///		Assembly reference object from the calling scope.
		/// </param>
		public void AddReference(
			Assembly assembly)
		{
			references.Add(assembly.Location);

			initialized = false;
		}

		/// <summary>
		///		Allows namespaces to be added to expression execution context so
		///		code within expression does not have to fully qualify classes.
		/// </summary>
		/// <param name="usingNamespace">
		///		Fully qualified namespace to be added to the expression execution
		///		context, e.g. "System.Collections.Generic".
		/// </param>
		public void AddUsing(
			string usingNamespace)
		{
			usings.Add(usingNamespace);

			initialized = false;
		}

		/// <summary>
		///		Allows a method definition to be added to the expression execution
		///		context and referenced within the expression.
		/// </summary>
		/// <param name="methodDefinition">
		///		Full definition of the method to add to the expression execution
		///		context. Can be public or private, static or non-static.
		/// </param>
		public void AddMethod(
			string methodDefinition)
		{
			methods.Add(methodDefinition);

			initialized = false;
		}

		private void InitEval(string caller)
		{
			Init(
				caller,
				hasReturn: true);
		}

		private void InitExec(string caller)
		{
			Init(
				caller,
				hasReturn: false);
		}

		private void Init(
			string caller,
			bool hasReturn)
		{
			CSharpCodeWriter writer = new CSharpCodeWriter();

			string classText = writer.GetClassText(
				expression,
				variables.Select(entry => new CSharpCodeWriter.Variable
				{
					Name = entry.Key,
					Type = entry.Value.Type
				}).ToList(),
				usings,
				methods,
				withReturn: hasReturn);

			if (DebugFileOutputName != null)
			{
				File.WriteAllText(
					DebugFileOutputName,
					classText);
			}

			// instead of taking the everytime hit of a synchronized lock
			// choosing to take the infrequent possible hit of simultaneous
			// calls creating multiple types with the same class text
			// only the last one's definition will be cached for the next caller
			bool alreadyCompiled = compiledTypes.ContainsKey(classText);

			if (alreadyCompiled)
			{
				execution = compiledTypes[classText];

				if (execution.Exception != null)
				{
					throw execution.Exception;
				}
			}
			else
			{
				references.Add(caller);

				// add references to containing assemblies for all used variable types
				variables
					.Select(v => v.Value.Type.Assembly.Location)
					.Distinct()
					.ToList()
					.ForEach(a => references.Add(a));

				execution = new Execution();

				Compiler compiler = new Compiler();

				Type newType;

				try
				{
					newType = compiler.Compile(
						classText,
						references,
						"EvalAssembly",
						"CustomEvaluator");
				}
				catch (CompilationException ex)
				{
					execution.Exception = ex;

					compiledTypes[classText] = execution;

					throw;
				}

				execution.Constructor = new DefaultClassConstructorExpression().GetFunc(
					newType);

				foreach (string key in variables.Keys)
				{
					Variable variable = variables[key];

					Func<object, object> getter = null;
					Action<object, object> setter = null;

					if (variable.Type.IsPublic)
					{
						getter = new GetInstanceMemberValueExpression().GetFunc(
							newType,
							key);

						setter = new SetInstanceMemberValueExpression().GetAction(
							newType,
							key);
					}
					else
					{
						getter = new WrapperTranslator().GetGetAndUnwrap(
							newType,
							key);

						setter = new WrapperTranslator().GetWrapAndSet(
							newType,
							key);
					}

					execution.Variables[key] = new ExecutionVariable
					{
						Getter = getter,
						Setter = setter
					};
				}

				if (hasReturn)
				{
					execution.Evaluate = new ExecuteInstanceMethodExpression().GetFuncWithReturn(
						newType,
						"Eval");
				}
				else
				{
					execution.Execute = new ExecuteInstanceMethodExpression().GetFuncWithNoReturn(
						newType,
						"Eval");
				}

				compiledTypes[classText] = execution;
			}

			initialized = true;
		}

		private object EvalInternal()
		{
			return EvalInternal(caller);
		}

		private object EvalInternal(string caller)
		{
			if (!initialized)
			{
				InitEval(caller);
			}

			object newObject = execution.Constructor();

			SetVariableValuesOnExecutionContext(
				newObject);

			object result = execution.Evaluate(
				newObject,
				new object[] { });

			GetVariableValuesFromExecutionContext(
				newObject);

			return result;
		}

		/// <summary>
		///		Executes the expression and returns the resulting value.
		/// </summary>
		/// <returns>
		///		Value specified to be returned from the expression.
		/// </returns>
		public object Eval()
		{
			if (!callerInitialized)
			{
				caller = Assembly.GetCallingAssembly().Location;
				callerInitialized = true;
			}

			return EvalInternal();
		}

		/// <summary>
		///		Executes the expression and returns the resulting value, cast as
		///		the specified object type.
		/// </summary>
		/// <typeparam name="T">
		///		Object type to cast the return value as.
		/// </typeparam>
		/// <returns>
		///		Value specified to be returned from the expression.
		/// </returns>
		public T Eval<T>()
		{
			if (!callerInitialized)
			{
				caller = Assembly.GetCallingAssembly().Location;
				callerInitialized = true;
			}

			object answer = EvalInternal();

			CastExpression<T> exp = new CastExpression<T>();
			Func<object, T> cast = exp.GetFunc();

			return cast(answer);
		}

		/// <summary>
		///		Simplified static method to execute an expression and return the
		///		resulting value.
		/// </summary>
		/// <param name="expression">
		///		C# based string expression to be evaluated, e.g. "return 1+1".
		/// </param>
		/// <returns>
		///		Value specified to be returned from the expression.
		/// </returns>
		public static object Eval(string expression)
		{
			string callerLocation = Assembly.GetCallingAssembly().Location;

			return new Evaluator(expression).EvalInternal(callerLocation);
		}

		/// <summary>
		///		Simplified static method to execute an expression and return the
		///		resulting value, cast as the specified object type.
		/// </summary>
		/// <typeparam name="T">
		///		Object type to cast the return value as.
		/// </typeparam>
		/// <param name="expression">
		///		C# based string expression to be evaluated, e.g. "return 1+1".
		/// </param>
		/// <returns>
		///		Value specified to be returned from the expression.
		/// </returns>
		public static T Eval<T>(string expression)
		{
			string callerLocation = Assembly.GetCallingAssembly().Location;

			return (T) new Evaluator(expression).EvalInternal(callerLocation);
		}

		/// <summary>
		///		Executes the expression without returning a value. Resulting
		///		new variable values that were updated inside the expression
		///		can be subsequently accessed using <see cref="GetVariable(string)"/>
		///		or <see cref="this[string]"/>. 
		/// </summary>
		public void Exec()
		{
			if (!callerInitialized)
			{
				caller = Assembly.GetCallingAssembly().Location;
				callerInitialized = true;
			}

			if (!initialized)
			{
				InitExec(caller);
			}

			object newObject = execution.Constructor();

			SetVariableValuesOnExecutionContext(
				newObject);

			execution.Execute(
				newObject,
				new object[] { });

			GetVariableValuesFromExecutionContext(
				newObject);
		}

		private void SetVariableValuesOnExecutionContext(
			object newObject)
		{
			if (execution.Variables.Count > 0)
			{
				foreach (var variable in execution.Variables)
				{
					Action<object, object> set = variable.Value.Setter;

					object variableValue = variables[variable.Key].Value;

					set(
						newObject,
						variableValue);
				}
			}
		}

		private void GetVariableValuesFromExecutionContext(
			object newObject)
		{
			if (execution.Variables.Count > 0)
			{
				foreach (var variable in execution.Variables)
				{
					Func<object, object> get = variable.Value.Getter;

					object variableValue = get(newObject);

					variables[variable.Key].Value = variableValue;
				}
			}
		}

		private sealed class Variable
		{
			public object Value = null;

			public Type Type = null;
		}

		private sealed class Execution
		{
			public Func<object, object[], object> Evaluate = null;

			public Action<object, object[]> Execute = null;

			public Func<object> Constructor = null;

			public Dictionary<string, ExecutionVariable> Variables = new Dictionary<string, ExecutionVariable>();

			public CompilationException Exception = null;
		}

		private sealed class ExecutionVariable
		{
			public Action<object, object> Setter = null;

			public Func<object, object> Getter = null;
		}
	}
}
