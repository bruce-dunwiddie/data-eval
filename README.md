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

### Simple Addition

Code:
```csharp
Console.WriteLine(Evaluator.Eval("return 1+1"));
```

Result:
```
2
```

[.Net Fiddle](https://dotnetfiddle.net/DTLu6Z)

### Variable Addition

Code:
```csharp
var evaluator = new Evaluator("return x+y");
evaluator["x"] = 1;
evaluator["y"] = 2;
Console.WriteLine("Result: " + evaluator.Eval());
```

Result:
```
Result: 3
```

[.Net Fiddle](https://dotnetfiddle.net/19moI3)

### Variable Updates With No Return

Code:
```csharp
var evaluator = new Evaluator("x++");
evaluator["x"] = 1;
evaluator.Exec();
Console.WriteLine("Result: " + evaluator["x"]);
```

Result:
```
Result: 2
```

[.Net Fiddle](https://dotnetfiddle.net/K30Ht3)

### Expressions With External Code

Code:
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

Result:
```
Hello World
```

### Add Callable Method

Code:
```csharp
Evaluator eval = new Evaluator("return IsFutureDate(date)");

eval.AddMethod(@"
	bool IsFutureDate(string date)
	{
		DateTime parsedDate;

		bool parsed = DateTime.TryParse(date, out parsedDate);

		return parsed && parsedDate > DateTime.Now;
	}");

eval["date"] = "1/1/1900";

Console.WriteLine(eval.Eval<bool>());
```

Result:
```
False
```

[.Net Fiddle](https://dotnetfiddle.net/zHq8VW)

### Speed

For maximal speed, instances of the Evaluator class should be created with static strings referencing variables and be reused by just changing the value of the referenced variables, instead of dynamically changing the expression or creating new Evaluator instances, e.g.

Code:
```csharp
List<Customer> customers = new List<Customer>()
{
	new Customer()
	{
		ID = 1,
		FirstName = "John",
		LastName = "Smith",
		OrderCount = 6,
		TotalSales = 75000
	},
	new Customer()
	{
		ID = 2,
		FirstName = "Bob",
		LastName = "Jones",
		OrderCount = 3,
		TotalSales = 25000
	}
};

Evaluator highValuedCheck = new Evaluator(
	// can now have this rule be editable outside of the application
	"return customer.OrderCount >= 5 || customer.TotalSales >= 100000");

List<Customer> highValuedCustomers = customers.Where(
	c => {

		highValuedCheck["customer"] = c;

		return highValuedCheck.Eval<bool>();

	}).ToList();
```

#### .Net Framework

- 18 million evaluations per second
- 4.5 million evaluations per second with one referenced variable
- 50 compilations of new expressions per second
- 200 thousand evaluations per second when creating new Evaluator instances each time but on a previously seen expression

#### .Net Core

- 28 million evaluations per second
- 4 million evaluations per second with one referenced variable
- 50 compilations of new expressions per second
- 400 thousand evaluations per second when creating new Evaluator instances each time but on a previously seen expression
