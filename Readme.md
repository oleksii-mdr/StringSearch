Description
-----------

This project provides C# implementation of Rabin-Karp multi pattern string search
algorithm.

Usage
-----

```csharp
string input = "test 123string456 my foo bar";

IMultiPatternSearch searcher = new RabinKarpSearch();
searcher.Init(new List<string> { "string", "foo" });

var matches = uut.FindAll(input);

foreach(var match in matches)
{
	Console.WriteLine("Start position: " + match.Item1);
	Console.WriteLine("Pattern string: " + match.Item2);
}
```

See tests for more examples

License
-------
This project is licensed ubder [Creative Commons 3 Unported](http://creativecommons.org/licenses/by/3.0/legalcode)
Put simply you are free

+ share to copy, distribute and transmit the work
+ remix to adapt the work
+ make commercial use of the work

Under the following conditions

+ Attribute the original work of the author

This project makes use of NUnit for unit testing, see license in the /lib/NUnit-2.6.1 folder
