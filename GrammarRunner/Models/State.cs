namespace GrammarRecognizer.Models;

public class State
{
  public string Name;
  public bool IsFinal;
  public Dictionary<char, List<State>> Transitions { get; }
  public List<State> EmptyTransitions { get; }

  public State(string name, bool isFinal)
  {
    Name = name;
    IsFinal = isFinal;
    Transitions = [];
    EmptyTransitions = [];
  }

  public void AddTransition(char symbol, State state)
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

  public void AddTransition(char symbol, List<State> states)
  {
    if (!Transitions.ContainsKey(symbol))
    {
      Transitions[symbol] = states;
      return;
    }

    foreach (State state in states)
    {
      if (!Transitions[symbol].Contains(state))
      {
        Transitions[symbol].Add(state);
      }
    }

  }

  public void AddEmptyTransition(State state)
  {
    if (EmptyTransitions.Contains(state))
    {
      return;
    }

    EmptyTransitions.Add(state);
  }

  public void RemoveEmptyTransitions()
  {
    List<State> reacheable = GetAllEmptyTransitions();

    if (reacheable.Any(s => s.IsFinal))
    {
      IsFinal = true;
    }

    foreach (var state in reacheable)
    {
      foreach ((char symbol, List<State> states) in state.Transitions)
      {
        AddTransition(symbol, states);
      }
    }
  }

  private List<State> GetAllEmptyTransitions()
  {
    Queue<State> queue = new(EmptyTransitions);
    List<State> visited = [.. EmptyTransitions];

    while (queue.Count != 0)
    {
      State current = queue.Dequeue();

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
