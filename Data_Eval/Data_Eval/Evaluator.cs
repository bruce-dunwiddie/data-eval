using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Data.Eval.CodeWriting;
using Data.Eval.Compilation;
using Data.Eval.Invocation.Expressions;

namespace Data.Eval
{
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

		public Evaluator(
			string expression)
		{
			this.expression = expression;
		}

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

		public void SetVariable(
			string name,
			object value)
		{
			// TODO: check variable naming standards

			if (variables.ContainsKey(name))
			{
				variables[name].Value = value;
			}
			else
			{
				variables[name] = new Variable
				{
					Type = value.GetType(),
					Value = value
				};
			}
		}

		public object GetVariable(
			string name)
		{
			return variables[name].Value;
		}

		public void AddReference(
			string assemblyPath)
		{
			references.Add(assemblyPath);
		}

		public void AddReference(
			Assembly assembly)
		{
			references.Add(assembly.Location);
		}

		public void AddUsing(
			string usingNamespace)
		{
			usings.Add(usingNamespace);
		}

		public void AddMethod(
			string methodDefinition)
		{
			methods.Add(methodDefinition);
		}

		private void InitEval(string caller)
		{
			CSharpCodeWriter writer = new CSharpCodeWriter();

			string classText = writer.GetClassTextWithReturn(
				expression,
				variables.Select(entry => new CSharpCodeWriter.Variable
				{
					Name = entry.Key,
					Type = entry.Value.Type
				}).ToList(),
				usings,
				methods);

			// instead of taking the everytime hit of a synchronized lock
			// choosing to take the infrequent possible hit of simultaneous
			// calls creating multiple types with the same class text
			// only the last one's definition will be cached for the next caller
			bool alreadyCompiled = compiledTypes.ContainsKey(classText);

			if (alreadyCompiled)
			{
				execution = compiledTypes[classText];
			}
			else
			{
				references.Add(caller);

				execution = new Execution();

				Compiler compiler = new Compiler();

				Type newType = compiler.Compile(
					classText,
					references);

				execution.Constructor = new DefaultClassConstructorExpression().GetFunc(
					newType);

				foreach (string key in variables.Keys)
				{
					Func<object, object> getter = new GetInstanceMemberValueExpression().GetFunc(
						newType,
						key);

					Action<object, object> setter = new SetInstanceMemberValueExpression().GetAction(
						newType,
						key);

					execution.Variables[key] = new ExecutionVariable
					{
						Getter = getter,
						Setter = setter,
						Type = variables[key].Type
					};
				}

				execution.Evaluate = new ExecuteInstanceMethodExpression().GetFuncWithReturn(
					newType,
					"Eval");

				compiledTypes[classText] = execution;
			}

			initialized = true;
		}

		private void InitExec(string caller)
		{
			CSharpCodeWriter writer = new CSharpCodeWriter();

			string classText = writer.GetClassTextWithNoReturn(
				expression,
				variables.Select(entry => new CSharpCodeWriter.Variable
				{
					Name = entry.Key,
					Type = entry.Value.Type
				}).ToList(),
				usings,
				methods);

			// instead of taking the everytime hit of a synchronized lock
			// choosing to take the infrequent possible hit of simultaneous
			// calls creating multiple types with the same class text
			// only the last one's definition will be cached for the next caller
			bool alreadyCompiled = compiledTypes.ContainsKey(classText);

			if (alreadyCompiled)
			{
				execution = compiledTypes[classText];
			}
			else
			{
				references.Add(caller);

				execution = new Execution();

				Compiler compiler = new Compiler();

				Type newType = compiler.Compile(
					classText,
					references);

				execution.Constructor = new DefaultClassConstructorExpression().GetFunc(
					newType);

				foreach (string key in variables.Keys)
				{
					Func<object, object> getter = new GetInstanceMemberValueExpression().GetFunc(
						newType,
						key);

					Action<object, object> setter = new SetInstanceMemberValueExpression().GetAction(
						newType,
						key);

					execution.Variables[key] = new ExecutionVariable
					{
						Getter = getter,
						Setter = setter,
						Type = variables[key].Type
					};
				}

				execution.Execute = new ExecuteInstanceMethodExpression().GetFuncWithNoReturn(
					newType,
					"Exec");

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

			object result = execution.Evaluate(
				newObject,
				new object[] { });

			if (execution.Variables.Count > 0)
			{
				foreach (var variable in execution.Variables)
				{
					Func<object, object> get = variable.Value.Getter;

					object variableValue = get(newObject);

					variables[variable.Key].Value = variableValue;
				}
			}

			return result;
		}

		public object Eval()
		{
			if (!callerInitialized)
			{
				caller = Assembly.GetCallingAssembly().Location;
				callerInitialized = true;
			}

			return EvalInternal();
		}

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

		public static object Eval(string expression)
		{
			string callerLocation = Assembly.GetCallingAssembly().Location;

			return new Evaluator(expression).EvalInternal(callerLocation);
		}

		public static T Eval<T>(string expression)
		{
			string callerLocation = Assembly.GetCallingAssembly().Location;

			return (T) new Evaluator(expression).EvalInternal(callerLocation);
		}

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

			execution.Execute(
				newObject,
				new object[] { });

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
		}

		private sealed class ExecutionVariable
		{
			public Action<object, object> Setter = null;

			public Func<object, object> Getter = null;

			public Type Type = null;
		}
	}
}
