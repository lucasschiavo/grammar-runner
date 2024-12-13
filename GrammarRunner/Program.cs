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

    var grammarReader = new GrammarReader(grammarFile);

    Grammar grammar = grammarReader.Read();

    DFAutomaton automaton = grammar
      .ToAutomaton()
      .ToDeterministic();

    Console.WriteLine(automaton.Compute("lucas@ufrgs.com.br"));
  }
}
