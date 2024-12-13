using GrammarRunner.Utils;

namespace GrammarRunner.Models;

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
