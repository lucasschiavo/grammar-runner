using GrammarRunner.Models;
using GrammarRunner.Parsing;

namespace GrammarRunner;

internal class Program
{
  static void Main(string[] args)
  {
    if (args.Length == 0)
    {
      Console.WriteLine("Error: Enter a file path!");
      return;
    }

    var grammarReader = new GrammarReader(args[0]);

    Grammar grammar = grammarReader.Read();

    DFAutomaton automaton = grammar
      .ToAutomaton()
      .ToDeterministic();

    Console.WriteLine(automaton.Compute("lucas@ufrgs.com.br"));
  }
}
