using System;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

using Data.Eval;

namespace ExamplesFramework
{
	public class Program
	{
		static void Main(string[] args)
		{
			// prime libraries
			Console.WriteLine("priming");

			// these times are totally bogus while various libraries load
			int k = CSharpScript.EvaluateAsync<int>("10+2").Result;
			k = Evaluator.Eval<int>("return 10+");

			Console.WriteLine("primed");

			Console.WriteLine("initializing");

			Console.WriteLine("Data.Eval");
			Stopwatch timer = Stopwatch.StartNew();
			for (int i = 0; i < 10; i++)
			{
				int j = Evaluator.Eval<int>("return 1+" + i.ToString());
			}
			timer.Stop();
			Console.WriteLine(timer.ElapsedMilliseconds.ToString());

			Console.WriteLine("CSharpScript");
			timer = Stopwatch.StartNew();
			for (int i = 0; i < 10; i++)
			{
				int j = CSharpScript.EvaluateAsync<int>("1+" + i.ToString()).Result;
			}
			timer.Stop();
			Console.WriteLine(timer.ElapsedMilliseconds.ToString());

			Console.WriteLine("looping");

			Console.WriteLine("Data.Eval");
			var eval = new Evaluator("return 1+3");
			timer = Stopwatch.StartNew();
			for (int i = 0; i < 10000000; i++)
			{
				int j = eval.Eval<int>();
			}
			timer.Stop();
			Console.WriteLine(timer.ElapsedMilliseconds.ToString());

			Console.WriteLine("CSharpScript");
			var script = CSharpScript.Create<int>("1+3");
			//script.Compile();
			ScriptRunner<int> runner = script.CreateDelegate();
			timer = Stopwatch.StartNew();
			for (int i = 0; i < 10000000; i++)
			{
				//int j = script.RunAsync().Result.ReturnValue;
				int j = runner().Result;
			}
			timer.Stop();
			Console.WriteLine(timer.ElapsedMilliseconds.ToString());

			Console.WriteLine("looping with variables");

			Console.WriteLine("Data.Eval");
			eval = new Evaluator("return x+y");
			timer = Stopwatch.StartNew();
			for (int i = 0; i < 10000000; i++)
			{
				eval.SetVariable("x", i);
				eval.SetVariable("y", i);
				int j = eval.Eval<int>();
			}
			timer.Stop();
			Console.WriteLine(timer.ElapsedMilliseconds.ToString());

			Console.WriteLine("CSharpScript");
			script = CSharpScript.Create<int>("x+y", globalsType: typeof(Globals));
			//script.Compile();
			runner = script.CreateDelegate();
			timer = Stopwatch.StartNew();
			for (int i = 0; i < 10000000; i++)
			{
				//int j = script.RunAsync().Result.ReturnValue;
				int j = runner(new Globals { x = i, y = i }).Result;
			}
			timer.Stop();
			Console.WriteLine(timer.ElapsedMilliseconds.ToString());

			Console.ReadLine();
		}

		public class Globals
		{
			public int x;
			public int y;
		}
	}
}
