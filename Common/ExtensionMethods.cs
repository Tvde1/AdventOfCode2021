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

        public static IEnumerable<(TOut, TOut)> Pairs<TOut, TIn>(this IEnumerable<TIn> items, Func<TIn, TOut> mapFunc)
        {
            using var enumerator = items.GetEnumerator();
            if (!enumerator.MoveNext())
                yield break;

            var previous = mapFunc(enumerator.Current);
            while (enumerator.MoveNext())
            {
                var current = mapFunc(enumerator.Current);
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
            try
            {
                return source[x, y];
            }
            catch
            {
                return null;
            }
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

        public static ParallelQuery<T> WhereNotNull<T>(this ParallelQuery<T?> source)
            where T : class
        {
            return source.Where(x => x is not null).Cast<T>();
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
            var index = (int)Math.Floor(list.Count / 2d);
            var item = list[index];
            return item;
        }

        public static T Middle<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            var index = (int)Math.Floor(list.Count / 2d);
            var item = list[index];
            return item;
        }

        public static Dictionary<TKey, TValue> UpdateOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, Func<TValue, TValue> updateFunc, TValue initialValue)
            where TKey : notnull
        {
            if (source.TryGetValue(key, out var value))
            {
                source[key] = updateFunc(value);
            }
            else
            {
                source.Add(key, initialValue);
            }

            return source;
        }

        public static IEnumerable<T> Flatten<T>(this T[,] source)
            where T : struct
        {
            return source.Cast<T>();
        }

        public static int MostCommonCount<T>(this IEnumerable<T> source)
        {
            return source.GroupBy(x => x).MaxBy(x => x.Count())!.Count();
        }

        public static int LeastCommonCount<T>(this IEnumerable<T> source)
        {
            return source.GroupBy(x => x).MinBy(x => x.Count())!.Count();
        }

        public static string Join(this (char, char) pair)
        {
            return pair.Item1.ToString() + pair.Item2;
        }

        public static string Join(this (char, char, char) triplet)
        {
            return triplet.Item1.ToString() + triplet.Item2 + triplet.Item3;
        }
    }
}