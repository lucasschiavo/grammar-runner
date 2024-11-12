using GrammarRecognizer.Models;
using System.Text.RegularExpressions;

namespace GrammarRecognizer.Parsing;

public partial class GrammarReader
{
  private readonly Stack<string> _lines;

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

    var variables = grammarDefinition[2].Value
      .Split(",")
      .Select(v => v[0])
      .ToArray();

    var terminals = grammarDefinition[3].Value
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
      string production = ruleMatch.Groups[2].Value;

      rules.Add(new Rule(var, production));
    }

    return new Grammar(
      name,
      variables,
      terminals,
      [.. rules],
      initialVariable
    );
  }

  [GeneratedRegex(@"\s+")]
  private static partial Regex WhitespacePattern();

  [GeneratedRegex(@"([a-zA-Z0-9_]+)=\({([a-zA-Z,]+)},{([a-zA-Z,]+)},P,([a-zA-Z])\)")]
  private static partial Regex GrammarTuplePattern();

  // valid for unitary grammars
  [GeneratedRegex(@"([a-zA-Z]{1})->([a-zA-Z]{0,2})")]
  private static partial Regex GrammarRulePattern();
}
