using System;
using System.Collections.Generic;
using System.Linq;

using Data.Eval.CodeWriting;
using Data.Eval.Compilation;
using Data.Eval.Invocation.Expressions;

namespace Data.Eval
{
	public sealed class Evaluator
	{
		private string expression;
		private Dictionary<string, Variable> variables = null;
		private bool initialized = false;
		private Execution execution = null;

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

			if (variables == null)
			{
				variables = new Dictionary<string, Variable>();
			}

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

		private void InitEval()
		{
			CSharpCodeWriter writer = new CSharpCodeWriter();

			string classText = writer.GetClassTextWithReturn(
				expression,
				variables == null ?
					new List<CSharpCodeWriter.Variable> { } :
					variables.Select(entry => new CSharpCodeWriter.Variable
					{
						Name = entry.Key,
						Type = entry.Value.Type
					}).ToList());

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
				execution = new Execution();

				Compiler compiler = new Compiler();

				Type newType = compiler.Compile(
					classText);

				execution.Constructor = new DefaultClassConstructorExpression().GetFunc(
					newType);

				if (variables != null)
				{
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
				}

				execution.Evaluate = new ExecuteInstanceMethodExpression().GetFuncWithReturn(
					newType,
					"Eval");

				compiledTypes[classText] = execution;
			}

			initialized = true;
		}

		private void InitExec()
		{
			CSharpCodeWriter writer = new CSharpCodeWriter();

			string classText = writer.GetClassTextWithNoReturn(
				expression,
				variables == null ?
					new List<CSharpCodeWriter.Variable> { } :
					variables.Select(entry => new CSharpCodeWriter.Variable
					{
						Name = entry.Key,
						Type = entry.Value.Type
					}).ToList());

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
				execution = new Execution();

				Compiler compiler = new Compiler();

				Type newType = compiler.Compile(
					classText);

				execution.Constructor = new DefaultClassConstructorExpression().GetFunc(
					newType);

				if (variables != null)
				{
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
				}

				execution.Execute = new ExecuteInstanceMethodExpression().GetFuncWithNoReturn(
					newType,
					"Exec");

				compiledTypes[classText] = execution;
			}

			initialized = true;
		}

		public object Eval()
		{
			if (!initialized)
			{
				InitEval();
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

		public T Eval<T>()
		{
			object answer = Eval();

			CastExpression<T> exp = new CastExpression<T>();
			Func<object, T> cast = exp.GetFunc();

			return cast(answer);
		}

		public static object Eval(string expression)
		{
			return new Evaluator(expression).Eval();
		}

		public static T Eval<T>(string expression)
		{
			return new Evaluator(expression).Eval<T>();
		}

		public void Exec()
		{
			if (!initialized)
			{
				InitExec();
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

			public Type Type = null;

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
