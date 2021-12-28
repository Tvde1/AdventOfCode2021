using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AdventOfCode.Common.Models;
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

        public static T[,] ToTwoDimensionalArrayOld<T>(this IEnumerable<IEnumerable<T>> enumerable)
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

        public static T[,] ToTwoDimensionalArray<T>(this IEnumerable<IEnumerable<T>> enumerable)
        {
            var data = enumerable.Select(inner => inner.ToArray()).ToArray();

            var length = data.Length;
            var minorLength = data[0].Length;

            var arr = new T[length, minorLength];

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < minorLength; j++)
                {
                    arr[i, j] = data[i][j];
                }
            }

            return arr;
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

        [DebuggerStepThrough]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetPoint<T>(this T[,] grid, Point2D point)
        {
            return grid[point.Y, point.X];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetPointOr<T>(this T[,] grid, Point2D point, T or)
        {
            try
            {
                return grid[point.Y, point.X];
            }
            catch
            {
                return or;
            }
        }

        public static bool TryGetFirst<T>(this IEnumerable<T> items, Func<T, bool> predicate, out T item)
            where T : struct
        {
            foreach (var it in items)
            {
                if (!predicate(it))
                {
                    continue;
                }
                item = it;
                return true;
            }

            item = default;
            return false;
        }

        public static IEnumerable<bool> ToBits(this byte source, int mostSignificantPosition)
        {
            for (var i = mostSignificantPosition - 1; i >= 0; i--)
            {
                var toCheck = 1 << i;
                var isThat = (source & toCheck) == toCheck;

                yield return isThat;
            }
        }

        [DebuggerStepThrough]
        public static bool TryDequeueAmount<T>(this Queue<T> source, int amount, [NotNullWhen(true)] out T[]? items)
        {
            var list = new List<T>(amount);

            for (var i = 0; i < amount; i++)
            {
                if (!source.TryDequeue(out var item))
                {
                    items = null;
                    return false;
                }

                list.Add(item);

            }

            items = list.ToArray();

            return true;
        }

        [DebuggerStepThrough]
        public static int ArrangeBits(this IEnumerable<bool> source)
        {
            var result = 0;
            foreach (var b in source)
            {
                result <<= 1;
                if (b)
                {
                    result |= 1;
                }
            }

            return result;
        }

        public static void Fill<T>(this T[,] source, T value)
        {
            for (var x = 0; x < source.GetLength(0); x++)
                for (var y = 0; y < source.GetLength(1); y++)
                {
                    source[x, y] = value;
                }
        }

        public static T[,] Paste<T>(this T[,] source, T[,] toPaste, int xOffset = 0, int yOffset = 0)
        {
            for (var y = 0; y < toPaste.GetLength(0); y++)
                for (var x = 0; x < toPaste.GetLength(1); x++)
                    source[y + yOffset, x + xOffset] = toPaste[y, x];

            return source;
        }

        public static string Render<T>(this T[,] source, Func<T, string> toString)
        {
            var sb = new StringBuilder();
            for (var y = 0; y < source.GetLength(1); y++)
            {
                for (var x = 0; x < source.GetLength(0); x++)
                {
                    sb.Append(toString(source[x, y]));
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string Render<T>(this T[,] source, Func<T, char> toChar)
        {
            var sb = new StringBuilder();
            for (var y = 0; y < source.GetLength(0); y++)
            {
                for (var x = 0; x < source.GetLength(1); x++)
                {
                    sb.Append(toChar(source[y, x]));
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static T? Map<T>(this T? source, Func<T, T> selector)
            where T : struct
        {
            if (source.HasValue)
            {
                return selector(source.Value);
            }
            else
            {
                return null;
            }
        }

        public static T OrElse<T>(this T? source, T orElse)
            where T : struct
        {
            return source ?? orElse;
        }

        public static bool Contains(this Range range, int integer)
        {
            return range.Start.Value <= integer && range.End.Value >= integer;
        }
    }
}