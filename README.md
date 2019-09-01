# data-eval
.Net Library for Evaluating Expressions at Runtime

The goal of this library is to set up a fully featured easy interface for working with C# based expressions as strings with access to as much C# and .Net functionality as possible, with the highest performance and compatibility as possible.

String expressions allow application functionality to be added and modified outside of the application, even viewed and possibly edited by business users with limited technical knowledge.

Available on Nuget, [Data.Eval](https://www.nuget.org/packages/Data.Eval/).

    Install-Package Data.Eval


[![NuGet](https://img.shields.io/nuget/dt/Data.Eval.svg)](https://www.nuget.org/packages/Data.Eval/)

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

[![Build status](https://ci.appveyor.com/api/projects/status/ewhl0xxqok5yeqr3?svg=true)](https://ci.appveyor.com/project/bruce-dunwiddie/data-eval)

[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=data-eval&metric=coverage)](https://sonarcloud.io/component_measures?id=data-eval&metric=coverage)

[![Code Quality](https://sonarcloud.io/api/project_badges/measure?project=data-eval&metric=alert_status)](https://sonarcloud.io/dashboard?id=data-eval)

### Simple Addition

```csharp
Console.WriteLine(Evaluator.Eval("return 1+1"));
```

[.Net Fiddle](https://dotnetfiddle.net/DTLu6Z)

### Variable Addition

```csharp
var evaluator = new Evaluator("return x+y");
evaluator["x"] = 1;
evaluator["y"] = 2;
Console.WriteLine("Result: " + evaluator.Eval());
```

[.Net Fiddle](https://dotnetfiddle.net/19moI3)

### Variable Updates With No Return

```csharp
var evaluator = new Evaluator("x++");
evaluator["x"] = 1;
evaluator.Exec();
Console.WriteLine("Result: " + evaluator["x"]);
```

[.Net Fiddle](https://dotnetfiddle.net/K30Ht3)

### Expressions With External Code

```csharp
// you can reference external code from inside an expression
var eval = new Evaluator("message = ExampleClass.HelloWorld");

// add a reference to the external code by specifying the path to the dll
eval.AddReference(typeof(TestExternalReference.ExampleClass).Assembly.Location);

// can add usings to simplify expression
eval.AddUsing("TestExternalReference");

// define the holder variable by assigning it a default value
eval["message"] = "";

// execute expression and set the variable
eval.Exec();

// write out Hello World
Console.WriteLine(eval["message"]);
```
