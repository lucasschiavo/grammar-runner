namespace GrammarRecognizer.Models;

using GrammarRecognizer.Helpers;
using System.Text;

public class NDAutomaton
{
  public NDState InitialState;
  public List<NDState> States;

  public NDAutomaton(NDState initialState, List<NDState> states)
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
    List<NDState> currentStates = [InitialState];
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
    List<NDState> currentStates = [InitialState];
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

  public DFAutomaton ToDeterministic()
  {
    var unprocessedDStates = new Queue<List<NDState>>();
    var processedDStates = new HashSet<DState>();

    unprocessedDStates.Enqueue([InitialState]);

    while (unprocessedDStates.Count > 0)
    {
      var currentNDStates = unprocessedDStates.Dequeue();

      DState dState = processedDStates
        .Where(ds => ds.NDStates.SetEquals(currentNDStates))
        .FirstOrDefault() ?? new(currentNDStates);

      processedDStates.Add(dState); // redundante em alguns casos, melhorar logica

      foreach (var symbol in Alphabet())
      {
        var nextNDStates = new List<NDState>();

        foreach (var state in currentNDStates)
        {
          if (state.Transitions.ContainsKey(symbol))
          {
            nextNDStates.AddRange(state.Transitions[symbol]);
          }
        }

        if (nextNDStates.Count > 0)
        {
          var nextDState = processedDStates
            .Where(ds => ds.NDStates.SetEquals(nextNDStates))
            .FirstOrDefault() ?? new DState(nextNDStates);
          dState.AddTransition(symbol, nextDState);

          if (processedDStates.Add(nextDState))
          {
            unprocessedDStates.Enqueue(nextNDStates);
          }
        }
      }
    }
    var initialDState = processedDStates
      .Where(ds => ds.NDStates.SetEquals([InitialState]))
      .First();

    return new DFAutomaton(initialDState, processedDStates);
  }

  private HashSet<char> Alphabet()
  {
    var alphabet = new HashSet<char>();

    foreach (var state in States)
    {
      foreach (var transition in state.Transitions)
      {
        alphabet.Add(transition.Key);
      }
    }

    return alphabet;
  }
}


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
