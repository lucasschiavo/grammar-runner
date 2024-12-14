using GrammarRunner.Models;
using GrammarRunner.Parsing;

namespace GrammarRunner;

internal class Program
{
  static void Main(string[] args)
  {
    if (args.Length == 0)
    {
      Console.WriteLine("Usage: dotnet run -G=<grammar-file> -W=<words-file>");
      return;
    }

    var grammarArg = args.Where(s => s.StartsWith("-G=")).FirstOrDefault();
    var wordsArg = args.Where(s => s.StartsWith("-W=")).FirstOrDefault();

    if (grammarArg == null || wordsArg == null)
    {
      Console.WriteLine("Usage: dotnet run -G=<grammar-file> -W=<words-file>");
      return;
    }

    var grammarFile = grammarArg[3..];
    var wordsFile = wordsArg[3..];

    Grammar grammar = new GrammarReader(grammarFile).Read();

    DFAutomaton automaton = grammar
      .ToAutomaton()
      .ToDeterministic();

    string[] words = File.ReadAllLines(wordsFile);

    foreach (string word in words)
    {
      if (automaton.Compute(word))
      {
        Console.WriteLine($"✔️  {word} é aceito pela gramática.");
      }
      else
      {
        Console.WriteLine($"❌ {word} não é aceito pela gramática.");
      }
    }
  }
}
