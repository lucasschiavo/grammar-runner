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

public class DState
{
  public string Name;
  public bool IsFinal;
  public Dictionary<char, DState> Transitions { get; }
  public HashSet<State> NDStates;

  public DState(List<State> states)
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

public class DFAutomaton
{
  public DState InitialState;
  public List<DState> States;

  public DFAutomaton(DState initialState, List<DState> states)
  {
    InitialState = initialState;
    States = states;
  }
}
