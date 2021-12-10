using System;
using System.Collections.Generic;
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
			var o = TryCatch.Try(() => source[x, y]).Match(s => s, _ => (int?)null);
			return o;
		}

		public static string ToAlphabeticalOrder(this string source)
		{
			return new string(source.OrderBy(x => x).ToArray());
		}

		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
			where T : struct
		{
			return source.Where(x => x is not null).Select(x => x!.Value);
		}

		public static ParallelQuery<T> WhereNotNull<T>(this ParallelQuery<T?> source)
			where T : struct
		{
			return source.Where(x => x is not null).Select(x => x!.Value);
		}

		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
			where T : class
		{
			return source.Where(x => x is not null).Select(x => x!);
		}

		public static ParallelQuery<TLeft> WhereLeft<TLeft, TRight>(this ParallelQuery<Either<TLeft, TRight>> source)
		{
			return source.Where(x => x.IsLeft).Select(x => x.Match(l => l, _ => default!));
		}

		public static ParallelQuery<TRight> WhereRight<TLeft, TRight>(this ParallelQuery<Either<TLeft, TRight>> source)
		{
			return source.Where(x => x.IsRight).Select(x => x.Match(_ => default!, r => r));
		}

		public static IEnumerable<TLeft> WhereLeft<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> source)
		{
			return source.Where(x => x.IsLeft).Select(x => x.Match(l => l, _ => default!));
		}

		public static IEnumerable<TRight> WhereRight<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> source)
		{
			return source.Where(x => x.IsRight).Select(x => x.Match(_ => default!, r => r));
		}

		public static T Middle<T>(this ParallelQuery<T> source)
		{
			var list = source.ToList();
			int index = (int)Math.Floor(list.Count / 2d);
			var item = list[index];
			return item;
		}

		public static T Middle<T>(this IEnumerable<T> source)
		{
			var list = source.ToList();
			int index = (int)Math.Floor(list.Count / 2d);
			var item = list[index];
			return item;
		}
	}
}