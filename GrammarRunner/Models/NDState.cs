namespace GrammarRunner.Models;

public class NDState
{
  public string Name;
  public bool IsFinal;
  public Dictionary<char, List<NDState>> Transitions { get; }
  public List<NDState> EmptyTransitions { get; }

  public NDState(string name, bool isFinal)
  {
    Name = name;
    IsFinal = isFinal;
    Transitions = [];
    EmptyTransitions = [];
  }

  public void AddTransition(char symbol, NDState state)
  {
    if (!Transitions.ContainsKey(symbol))
    {
      Transitions[symbol] = [state];
      return;
    }

    if (!Transitions[symbol].Contains(state))
    {
      Transitions[symbol].Add(state);
    }
  }

  public void AddTransition(char symbol, List<NDState> states)
  {
    if (!Transitions.ContainsKey(symbol))
    {
      Transitions[symbol] = states;
      return;
    }

    foreach (NDState state in states)
    {
      if (!Transitions[symbol].Contains(state))
      {
        Transitions[symbol].Add(state);
      }
    }

  }

  public void AddEmptyTransition(NDState state)
  {
    if (EmptyTransitions.Contains(state))
    {
      return;
    }

    EmptyTransitions.Add(state);
  }

  public void RemoveEmptyTransitions()
  {
    List<NDState> reacheable = GetAllEmptyTransitions();

    if (reacheable.Any(s => s.IsFinal))
    {
      IsFinal = true;
    }

    foreach (var state in reacheable)
    {
      foreach ((char symbol, List<NDState> states) in state.Transitions)
      {
        AddTransition(symbol, states);
      }
    }
  }

  private List<NDState> GetAllEmptyTransitions()
  {
    Queue<NDState> queue = new(EmptyTransitions);
    List<NDState> visited = [.. EmptyTransitions];

    while (queue.Count != 0)
    {
      NDState current = queue.Dequeue();

      foreach (var state in current.EmptyTransitions)
      {
        if (!visited.Contains(state))
        {
          queue.Enqueue(state);
          visited.Add(state);
        }
      }
    }

    return visited;
  }
}
