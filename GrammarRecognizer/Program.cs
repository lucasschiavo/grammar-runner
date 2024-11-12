using GrammarRecognizer.Models;
using GrammarRecognizer.Parsing;

namespace GrammarRecognizer;

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
  }
}

