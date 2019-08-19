# data-eval
.Net Library for Evaluating Expressions at Runtime

Available on Nuget, [Data.Eval](https://www.nuget.org/packages/Data.Eval/).

    Install-Package Data.Eval


[![NuGet](https://img.shields.io/nuget/dt/Data.Eval.svg)](https://www.nuget.org/packages/Data.Eval/)

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

[![Build status](https://ci.appveyor.com/api/projects/status/ewhl0xxqok5yeqr3?svg=true)](https://ci.appveyor.com/project/bruce-dunwiddie/data-eval)

### Simple Addition

```csharp
Evaluator.Eval("return 1+1")
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

### Variable Updates with No Return

```csharp
var evaluator = new Evaluator("x++");
evaluator["x"] = 1;
evaluator.Exec();
Console.WriteLine("Result: " + evaluator["x"]);
```

### Expressions with external code

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
