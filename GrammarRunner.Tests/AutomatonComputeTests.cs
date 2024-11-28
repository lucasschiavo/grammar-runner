using GrammarRecognizer.Models;

namespace GrammarRunner.Tests;

public class AutomatonComputeTests
{
  [Fact]
  public void DeterministicAutomaton()
  {
    // This automaton accepts words with an odd number of a's
    State q0 = new("q0", false);
    State q1 = new("q1", true);

    q0.AddTransition('a', q1);
    q0.AddTransition('b', q0);
    q1.AddTransition('a', q0);
    q1.AddTransition('b', q1);

    Automaton automaton = new(q0, [q0, q1]);

    // words with odd number of a's return true
    Assert.True(automaton.Compute("bab"));
    Assert.True(automaton.Compute("aaaaaaa"));

    // words with even number of a's return false
    Assert.False(automaton.Compute("baba"));
    Assert.False(automaton.Compute("bbbbbbbbb"));
    Assert.False(automaton.Compute(""));
  }

  [Fact]
  public void NonDeterministicAutomaton()
  {
    // this automaton accepts words that have either "aa" or "bb" as substring
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

    Automaton automaton = new(q0, [q0, q1, q2, qf]);

    Assert.True(automaton.Compute("babbab"));
    Assert.True(automaton.Compute("babaab"));
    Assert.True(automaton.Compute("bb"));
    Assert.True(automaton.Compute("aa"));

    Assert.False(automaton.Compute("ababab"));
    Assert.False(automaton.Compute("a"));
    Assert.False(automaton.Compute("b"));
    Assert.False(automaton.Compute(""));
  }

  [Fact]
  public void EmptyMovementAutomaton_Sufix()
  {
    // this automaton accepts word that have either "a" or "bb" or "ccc" as sufix
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

    var automaton = new Automaton(q0, [q0, q1, q2, q3, q4, q5, q6, qf]);

    Assert.True(automaton.Compute("a"));
    Assert.True(automaton.Compute("abcabcabca"));
    Assert.True(automaton.Compute("bb"));
    Assert.True(automaton.Compute("accccabb"));
    Assert.True(automaton.Compute("ccc"));
    Assert.True(automaton.Compute("abccc"));

    Assert.False(automaton.Compute("b"));
    Assert.False(automaton.Compute("cc"));
    Assert.False(automaton.Compute("abcc"));
    Assert.False(automaton.Compute("acacacb"));
    Assert.False(automaton.Compute(""));
  }
}
