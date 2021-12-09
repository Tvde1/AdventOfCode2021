using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AdventOfCode.Common.Monads;

namespace AdventOfCode.Common
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

		public static IEnumerable<(T, T, T)> Triplets<T>(this IEnumerable<T> items)
		{
			using var enumerator = items.GetEnumerator();
			if (!enumerator.MoveNext())
				yield break;

			var first = enumerator.Current;

			if (!enumerator.MoveNext())
				yield break;

			var second = enumerator.Current;

			while (enumerator.MoveNext())
			{
				var current = enumerator.Current;
				yield return (first, second, current);
				first = second;
				second = current;
			}
		}

		public static IEnumerable<T> Enumerate<T>(this T item)
		{
			yield return item;
		}

		public static IEnumerator<int> GetEnumerator(this Range range)
		{
			var start = range.Start.GetOffset(int.MaxValue);
			var end = range.End.GetOffset(int.MaxValue);
			var acc = end.CompareTo(start);
			var current = start;
			do
			{
				yield return current;
				current += acc;
			} while (current != end);
		}

        public static T[,] ToTwoDimensionalArray<T>(this IEnumerable<IEnumerable<T>> enumerable)
        {
            var columns = enumerable.Select(inner => inner.ToArray()).ToArray();
            var lineCount = columns.Max(columns => columns.Length);
            var twa = new T[lineCount, columns.Length];
            for (var columnIndex = 0; columnIndex < columns.Length; columnIndex++)
            {
                var line = columns[columnIndex];
                for (var lineIndex = 0; lineIndex < line.Length; lineIndex++)
                {
                    twa[lineIndex, columnIndex] = line[lineIndex];
                }
            }
            return twa;
        }

        public static int? TryGet(this int[,] source, int x, int y)
        {
			var o = TryCatch.Try(() => source[x, y]).Match(s => s, _ => (int?) null);
            return o;
        }

        public static string ToAlphabeticalOrder(this string source)
        {
            return new string(source.OrderBy(x => x).ToArray());
        }
	}
}