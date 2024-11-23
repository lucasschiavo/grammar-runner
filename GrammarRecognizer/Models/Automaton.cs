namespace GrammarRecognizer.Models;

using GrammarRecognizer.Helpers;

public class Automaton
{
  public State InitialState;

  public Automaton(State initialState)
  {
    InitialState = initialState;
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

  public State(string name, bool isFinal)
  {
    Name = name;
    IsFinal = isFinal;
    Transitions = [];
  }

  public void AddTransition(char symbol, State state)
  {
    if (!Transitions.ContainsKey(symbol))
    {
      Transitions[symbol] = [state];
      return;
    }

    Transitions[symbol].Add(state);
  }
}
