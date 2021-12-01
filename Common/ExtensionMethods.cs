using System.Collections.Generic;

namespace AdventOfCode.Puzzles
{
    public static class Extensions
    {
        public static IEnumerable<(T, T)> Pairs<T>(this IEnumerable<T> items)
        {
            using var enumerator = items.GetEnumerator();
            if (!enumerator.MoveNext())
                yield break;

            var previous = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                yield return (previous, current);
                previous = current;
            }
        }
    }
}