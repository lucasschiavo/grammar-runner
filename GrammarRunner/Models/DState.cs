namespace GrammarRunner.Models;

using GrammarRunner.Utils;

public class DState
{
  public string Name;
  public bool IsFinal;
  public Dictionary<char, DState> Transitions { get; }
  public HashSet<NDState> NDStates;

  public DState(List<NDState> states)
  {
    Name = states.Select(s => s.Name).StringJoin(",");
    IsFinal = states.Any(s => s.IsFinal);
    Transitions = [];
    NDStates = states.ToHashSet();
  }

  public void AddTransition(char symbol, DState state) => Transitions.Add(symbol, state);

  // DStates are compared by their ND States
  public override bool Equals(object? obj)
  {
    if (obj is DState other)
    {
      return NDStates.SetEquals(other.NDStates);
    }

    return false;
  }

  // https://stackoverflow.com/questions/371328/why-is-it-important-to-override-gethashcode-when-equals-method-is-overridden
  public override int GetHashCode()
  {
    int hash = 17;
    foreach (var state in NDStates)
    {
      hash = hash * 23 + state.Name.GetHashCode();
    }
    return hash;
  }
}
