using System.Text;

namespace GrammarRunner.Models;

public class DFAutomaton
{
  public DState InitialState;
  public HashSet<DState> States;

  public DFAutomaton(DState initialState, HashSet<DState> states)
  {
    InitialState = initialState;
    States = states;
  }

  public bool Compute(string word)
  {
    Stack<char> symbols = new(word.Reverse());
    DState? currentState = InitialState;

    while (symbols.Count > 0)
    {
      var symbol = symbols.Pop();
      currentState = currentState?.Transitions.GetValueOrDefault(symbol);
    }

    return currentState?.IsFinal ?? false;
  }

  public string ToDot()
  {
    var builder = new StringBuilder()
      .AppendLine("digraph G {")
      .AppendLine(@"invis[style=""invis""]")        // 
      .AppendLine($"invis -> {InitialState.Name}"); // invisible node that points to the initial state

    foreach (var fromState in States)
    {
      Dictionary<DState, List<char>> groupedTransitions = fromState.Transitions
        .GroupBy(s => s.Value)
        .ToDictionary(
          group => group.Key,
          group => group.Select(kv => kv.Key).ToList()
        );

      if (fromState.IsFinal)
      {
        builder = builder.AppendLine(@$"{fromState.Name}[shape=""doublecircle""]");
      }

      foreach ((DState toState, List<char> symbols) in groupedTransitions)
      {
        builder = builder.Append($"{fromState.Name} -> {toState.Name}")
          .Append(@"[label="" ")
          .AppendJoin(", ", symbols)
          .AppendLine(@"""]");
      }
    }

    builder = builder.AppendLine("}");

    return builder.ToString();
  }
}
