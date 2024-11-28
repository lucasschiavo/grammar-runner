﻿using GrammarRecognizer.Models;
using GrammarRecognizer.Parsing;

namespace GrammarRecognizer;

internal class Program
{
  static void Main(string[] args)
  {
    if (args.Length == 0)
    {
      Console.WriteLine("Error: Enter a file path!");
      return;
    }

    var grammarReader = new GrammarReader(args[0]);

    Grammar grammar = grammarReader.Read();

    Automaton automaton = grammar.ToAutomaton();

    /* DETERMINISTIC AUTOMATON
    var q0 = new State("q0", false);
    var q1 = new State("q1", true);

    q0.AddTransition('a', q1);
    q0.AddTransition('b', q0);
    q1.AddTransition('a', q0);
    q1.AddTransition('b', q1);

    var automaton = new Automaton(q0);

    automaton.PrettyCompute("baba");
    */

    /* NON DETERMINISTIC AUTOMATON
    State q0 = new("q0", false);
    State q1 = new("q1", false);
    State q2 = new("q2", false);
    State qf = new("qf", true);

    q0.AddTransition('a', q0);
    q0.AddTransition('b', q0);
    q0.AddTransition('a', q1);
    q0.AddTransition('b', q2);

    q1.AddTransition('a', qf);

    q2.AddTransition('b', qf);

    qf.AddTransition('a', qf);
    qf.AddTransition('b', qf);

    Automaton NDAutomaton = new(q0);

    NDAutomaton.PrettyCompute("ababaab");
    */

    /*
    State q0 = new("q0", false);
    State q1 = new("q1", false);
    State q2 = new("q2", false);
    State q3 = new("q3", false);
    State q4 = new("q4", false);
    State q5 = new("q5", false);
    State q6 = new("q6", false);
    State qf = new("qf", true);

    q0.AddEmptyTransition(q1);
    q0.AddEmptyTransition(q2);
    q0.AddEmptyTransition(q4);
    q0.AddTransition('a', q0);
    q0.AddTransition('b', q0);
    q0.AddTransition('c', q0);

    q1.AddTransition('a', qf);

    q2.AddTransition('b', q3);
    q3.AddTransition('b', qf);

    q4.AddTransition('c', q5);
    q5.AddTransition('c', q6);
    q6.AddTransition('c', qf);

    var emptyAutomaton = new Automaton(q0, [q0, q1, q2, q3, q4, q5, q6, qf]);

    emptyAutomaton.PrettyCompute("abbabbccc");
    */
  }
}

