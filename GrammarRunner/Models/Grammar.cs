namespace GrammarRunner.Models;

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

  public NDAutomaton ToAutomaton()
  {
    Dictionary<char, NDState> stateDict = [];
    NDState finalState = new("qf", true);

    // initialize all the automaton states
    foreach (char var in Productions.Keys)
    {
      stateDict.Add(var, new NDState($"q{var}", false));
    }

    foreach ((char var, List<Production> productions) in Productions)
    {
      // maps each production to a possibly empty transition
      foreach (Production production in productions)
      {
        if (production.Symbol == null && production.Variable == null)
        {
          stateDict[var].AddEmptyTransition(finalState);
        }
        else if (production.Variable != null && production.Symbol != null)
        {
          stateDict[var].AddTransition(production.Symbol.Value, stateDict[production.Variable.Value]);
        }
        else if (production.Symbol != null) // only has a terminal
        {
          stateDict[var].AddTransition(production.Symbol.Value, finalState);
        }
        else if (production.Variable != null) // only has a variable
        {
          stateDict[var].AddEmptyTransition(stateDict[production.Variable.Value]);
        }
      }
    }

    NDState initialState = stateDict[InitialVariable];

    return new NDAutomaton(initialState, [.. stateDict.Values]);
  }
}

public record Rule(char Variable, Production Production);

public record Production(char? Symbol, char? Variable);
