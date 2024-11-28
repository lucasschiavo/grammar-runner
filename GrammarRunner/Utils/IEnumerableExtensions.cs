namespace GrammarRecognizer.Helpers;

internal static class IEnumerableExtensions
{
  public static string StringJoin(this IEnumerable<string> values, string separator)
    => string.Join(separator, values);
}