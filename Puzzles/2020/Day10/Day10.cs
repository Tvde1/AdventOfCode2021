using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Puzzles._2020.Day10;

public class Day10 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"28
33
18
42
31
14
46
20
48
47
24
23
49
45
19
38
39
11
1
32
25
35
8
17
7
9
4
2
34
10
3");

    public Day10()
        : base(AdventDayImplementation.Build(TestInput, Parse, PartOne, PartTwo))
    { }

    private static int[] Parse(string input) => input.Split(Environment.NewLine).Select(int.Parse).ToArray();

    private static string PartOne(int[] data)
    {
        var differences = data
            .OrderBy(x => x)
            .Pairs()
            .Select(pair => pair.Item2 - pair.Item1)
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => x.Count() + 1);

        Console.WriteLine(string.Join(Environment.NewLine, differences));

        return (differences[1] * differences[3]).ToString();
    }

    private static string PartTwo(int[] data)
    {
        var chunks = new List<int[]>();

        var currentChunk = new List<int>();
        foreach (var number in data.OrderBy(x => x))
        {
            if (!currentChunk.Any())
            {
                currentChunk = new List<int> { number };
                continue;
            }

            if (number - currentChunk.Last() == 3)
            {
                chunks.Add(currentChunk.ToArray());
                currentChunk.Clear();
                continue;
            }

            currentChunk.Add(number);
        }
        chunks.Add(currentChunk.ToArray());

        var total = 1L;

        checked
        {
            total = chunks.Aggregate(total, (current, chunk) => current * CalculateVariations(chunk));
        }

        return total.ToString();
    }

    private static long CalculateVariations(int[] input)
    {
        return 1 + CalculateVariationsFrom(input[0], input[1..]);
    }

    private static long CalculateVariationsFrom(int start, IEnumerable<int> next)
    {
        var total = 0L;
        var current = start;
        foreach (var item in next)
        {
            if (item == current + 1)
            {
                current = item;
                total += 1;
            }

            if (item == current + 2 || item == current + 3)
            {
                total += 1;
                total += CalculateVariationsFrom(item, next);
            }
        }

        return total;
    }
}