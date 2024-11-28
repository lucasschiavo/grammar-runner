using GrammarRecognizer.Models;
using System.Text.RegularExpressions;

namespace GrammarRecognizer.Parsing;

public partial class GrammarReader
{
  private readonly Stack<string> _lines;

  private char[] Variables = [];
  private char[] Terminals = [];

  public GrammarReader(string filePath)
  {
    var lines = File.ReadLines(filePath)
      .Select(line => WhitespacePattern().Replace(line, ""))
      .Where(line => line.Length != 0)
      .Reverse()
      .ToArray();

    _lines = new Stack<string>(lines);
  }

  public Grammar Read()
  {
    var grammarDefinition = GrammarTuplePattern()
      .Match(_lines.Pop())
      .Groups;

    var name = grammarDefinition[1].Value;

    Variables = grammarDefinition[2].Value
      .Split(",")
      .Select(v => v[0])
      .ToArray();

    Terminals = grammarDefinition[3].Value
      .Split(",")
      .Select(v => v[0])
      .ToArray();

    var initialVariable = grammarDefinition[4].Value[0];

    _lines.Pop(); // P = {

    List<Rule> rules = [];

    while (_lines.Count != 1)
    {
      var ruleMatch = GrammarRulePattern().Match(_lines.Pop());

      char var = ruleMatch.Groups[1].Value[0];
      string productionString = ruleMatch.Groups[2].Value;

      var production = ParseProduction(productionString);

      rules.Add(new Rule(var, production));
    }

    return new Grammar(
      name,
      Variables,
      Terminals,
      [.. rules],
      initialVariable
    );
  }

  private Production ParseProduction(string productionString)
  {
    if (productionString.Length == 2)
    {
      char symbol = productionString[0];
      char variable = productionString[1];

      if (!Variables.Contains(variable))
      {
        throw new Exception($"Invalid production: {variable} is not a variable!");
      }

      if (!Terminals.Contains(symbol))
      {
        throw new Exception($"Invalid production: {symbol} is not a terminal!");
      }

      return new Production(symbol, variable);
    }
    else if (productionString.Length == 1)
    {
      var productionChar = productionString[0];

      if (Variables.Contains(productionChar))
      {
        return new Production(null, productionChar);
      }

      if (Terminals.Contains(productionChar))
      {
        return new Production(productionChar, null);
      }

      throw new Exception($"Invalid production: {productionChar}");
    }

    return new Production(null, null);
  }

  [GeneratedRegex(@"\s+")]
  private static partial Regex WhitespacePattern();

  [GeneratedRegex(@"([a-zA-Z0-9_]+)=\({([a-zA-Z,]+)},{([a-zA-Z0-9._\-@,]+)},P,([a-zA-Z])\)")]
  private static partial Regex GrammarTuplePattern();

  // valid for unitary grammars
  [GeneratedRegex(@"([a-zA-Z]{1})->([a-zA-Z0-9._\-@]{0,2})[,]?")]
  private static partial Regex GrammarRulePattern();
}
