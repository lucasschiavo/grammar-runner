using GrammarRecognizer.Models;

namespace GrammarRunner.Tests;

public class AutomatonComputeTests
{
  [Fact]
  public void DeterministicAutomaton()
  {
    // This automaton accepts words with an odd number of a's
    NDState q0 = new("q0", false);
    NDState q1 = new("q1", true);

    q0.AddTransition('a', q1);
    q0.AddTransition('b', q0);
    q1.AddTransition('a', q0);
    q1.AddTransition('b', q1);

    NDAutomaton automaton = new(q0, [q0, q1]);

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
    NDState q0 = new("q0", false);
    NDState q1 = new("q1", false);
    NDState q2 = new("q2", false);
    NDState qf = new("qf", true);

    q0.AddTransition('a', q0);
    q0.AddTransition('b', q0);
    q0.AddTransition('a', q1);
    q0.AddTransition('b', q2);

    q1.AddTransition('a', qf);

    q2.AddTransition('b', qf);

    qf.AddTransition('a', qf);
    qf.AddTransition('b', qf);

    NDAutomaton automaton = new(q0, [q0, q1, q2, qf]);

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
    NDState q0 = new("q0", false);
    NDState q1 = new("q1", false);
    NDState q2 = new("q2", false);
    NDState q3 = new("q3", false);
    NDState q4 = new("q4", false);
    NDState q5 = new("q5", false);
    NDState q6 = new("q6", false);
    NDState qf = new("qf", true);

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

    var automaton = new NDAutomaton(q0, [q0, q1, q2, q3, q4, q5, q6, qf]);

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

  [Fact]
  public void EmptyMovementAutomaton_ManyEmptyTransitions()
  {
    // this automaton recognized words of the language a*b*a*
    NDState q0 = new("q0", false);
    NDState q1 = new("q1", false);
    NDState q2 = new("q2", true);

    q0.AddEmptyTransition(q1);
    q0.AddTransition('a', q0);

    q1.AddEmptyTransition(q2);
    q1.AddTransition('b', q1);

    q2.AddTransition('a', q2);

    NDAutomaton automaton = new(q0, [q0, q1, q2]);

    Assert.True(automaton.Compute("aaaaabbbbbaaaaa"));
    Assert.True(automaton.Compute("bbbbbaaaaa"));
    Assert.True(automaton.Compute("aaaaabbbbb"));
    Assert.True(automaton.Compute("aaaaa"));
    Assert.True(automaton.Compute("bbbbb"));
    Assert.True(automaton.Compute(""));

    Assert.False(automaton.Compute("aabbbaaaabb"));
    Assert.False(automaton.Compute("bbbaaaabbaa"));
  }

  [Fact]
  public void DState_Equality()
  {
    // this automaton accepts words that have either "aa" or "bb" as substring
    NDState q0 = new("q0", false);
    NDState q1 = new("q1", false);
    NDState q2 = new("q2", false);
    NDState qf = new("qf", true);

    DState dq0 = new([q0, q1]);
    DState dq1 = new([q1, q0]);
    DState dq2 = new([q1, q2]);

    Assert.True(dq0.Equals(dq1));

    HashSet<DState> set1 = [dq1];

    Assert.Contains(dq1, set1);
    Assert.DoesNotContain(dq0, set1);
  }

  [Fact]
  public void NDAutomaton_ToDeterministic()
  {
    // this automaton accepts words that have "aaa" as sufix
    NDState q0 = new("q0", false);
    NDState q1 = new("q1", false);
    NDState q2 = new("q2", false);
    NDState qf = new("qf", true);

    q0.AddTransition('a', [q0, q1]);
    q0.AddTransition('b', q0);

    q1.AddTransition('a', q2);
    q2.AddTransition('a', qf);

    var automaton = new NDAutomaton(q0, [q0, q1, q2, qf]).ToDeterministic();

    Assert.True(automaton.States.Count >= 4);
  }
}
