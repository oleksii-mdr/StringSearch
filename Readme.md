Description
-----------

This project provides C# implementation of Rabin-Karp multi pattern string search
algorithm. 

I also hope to add more samples for string search in future (we all  have plans... sigh)

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

Want to contribute?
-------
You are welcome to contribute, feel free to make a pull request.
