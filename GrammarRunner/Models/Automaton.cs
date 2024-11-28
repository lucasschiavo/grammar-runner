namespace GrammarRecognizer.Models;

using GrammarRecognizer.Helpers;

public class Automaton
{
  public State InitialState;
  public List<State> States;

  public Automaton(State initialState, List<State> states)
  {
    InitialState = initialState;
    States = states;

    // empty movement automaton to non deterministic
    foreach (var state in States)
    {
      state.RemoveEmptyTransitions();
    }
  }

  public bool Compute(string word)
  {
    List<State> currentStates = [InitialState];
    Stack<char> symbols = new(word.Reverse());

    while (symbols.Count != 0 && currentStates.Count != 0)
    {
      char currentSymbol = symbols.Pop();

      currentStates = currentStates
        .SelectMany(s => s.Transitions.GetValueOrDefault(currentSymbol) ?? [])
        .ToList();
    }

    return currentStates.Any(s => s.IsFinal);
  }

  public bool PrettyCompute(string word)
  {
    List<State> currentStates = [InitialState];
    Stack<char> symbols = new(word.Reverse());

    Console.WriteLine($"Word: {word}");
    Console.WriteLine($"Initial State: {InitialState.Name}");
    Console.WriteLine("====================");

    while (symbols.Count != 0 && currentStates.Count != 0)
    {
      char currentSymbol = symbols.Pop();

      Console.Write($"δ*({{{currentStates.Select(s => s.Name).StringJoin(", ")}}}, {currentSymbol}) = ");

      currentStates = currentStates
        .SelectMany(s => s.Transitions.GetValueOrDefault(currentSymbol) ?? [])
        .ToList();

      Console.WriteLine($"{{{currentStates.Select(s => s.Name).StringJoin(", ")}}}");
    }

    var finalStates = currentStates.Where(s => s.IsFinal);

    Console.WriteLine("====================");

    if (finalStates.Any())
    {
      Console.WriteLine($"The automaton recognizes {word}");
      Console.WriteLine($"Final state: {finalStates.First().Name}");
      return true;
    }

    Console.WriteLine($"The automaton does not recognize {word}");
    return false;
  }
}

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
