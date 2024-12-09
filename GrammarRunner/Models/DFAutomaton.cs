namespace GrammarRecognizer.Models;

using System.Text;

public class DFAutomaton
{
  public DState InitialState;
  public HashSet<DState> States;

  public DFAutomaton(DState initialState, HashSet<DState> states)
  {
    InitialState = initialState;
    States = states;
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
