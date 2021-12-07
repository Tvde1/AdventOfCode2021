using System;
using System.Collections.Generic;
using System.Linq;

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
	}
}