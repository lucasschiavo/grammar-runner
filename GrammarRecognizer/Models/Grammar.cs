namespace GrammarRecognizer.Models;

public class Grammar(
  string name,
  char[] variables,
  char[] terminals,
  Rule[] rules,
  char initialVariable
)
{
  public string Name = name;
  public char[] Variables = variables;
  public char[] Terminals = terminals;
  public Rule[] Rules = rules;
  public char InitialVariable = initialVariable;
}

public record Rule(char Variable, string Production);
