namespace GrammarRecognizer.Models;

public class Grammar
{
  public string Name;
  public char[] Variables;
  public char[] Terminals;
  public Dictionary<char, List<Production>> Productions;
  public char InitialVariable;

  public Grammar
  (
    string name,
    char[] variables,
    char[] terminals,
    Rule[] rules,
    char initialVariable
  )
  {
    Name = name;
    Variables = variables;
    Terminals = terminals;
    Productions = rules
      .GroupBy(r => r.Variable)
      .ToDictionary(group => group.Key, group => group.Select(kv => kv.Production).ToList());
    InitialVariable = initialVariable;
  }
}

public record Rule(char Variable, Production Production);

public record Production(char? Symbol, char? Variable);
