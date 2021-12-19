using AdventOfCode.Common;
using AdventOfCode.Common.Monads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Puzzles.Day18;

public class Day18 : AdventDay
{
    private const string InputFile = "Day18/day18.txt";

    private const string TestInput = @"[[[[4,3],4],4],[7,[[8,4],9]]]
[1,1]";

    private const string TestInput2 = @"[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]";

    public Day18()
        : base(18, AdventDayImplementation.Build(AdventDataSource.FromRaw(TestInput), Parse, PartOne))
    { }

    private static SnailFishNumber[] Parse(string input) => input.Split(Environment.NewLine).Select(SnailFishNumber.Parse).ToArray();

    private static string PartOne(SnailFishNumber[] data)
    {
        var number = data[0];

        Console.WriteLine($"Initial number: {number}");
        Console.WriteLine();

        ReduceUntil(number);

        foreach (var snailFishNumber in data.Skip(1))
        {
            Console.WriteLine("  " + number);
            Console.WriteLine(" +" + snailFishNumber);

            number = SnailFishNumber.Add(number, snailFishNumber);
            Console.WriteLine(" =" + number);

            ReduceUntil(number);

            Console.WriteLine();
            // Debug.Assert(reduceResult is CompletedReduceOperation);
        }

        return number.CalculateMagnitude().ToString();
    }

    private static void ReduceUntil(SnailFishNumber number)
    {
        ReduceOperation? reduceResult = null;
        var i = 1;
        while ((reduceResult = number.Reduce()) is not NoActionReduceOperation)
        {
            Console.WriteLine($"After step {i:D2}: {number}");
            i++;
        }
    }

    private static string PartTwo(SnailFishNumber[] data) => data.ToString()!;
}