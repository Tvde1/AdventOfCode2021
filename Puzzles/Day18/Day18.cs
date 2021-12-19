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

    private const string TestInput = @"[1,2]
[[1,2],3]
[9,[8,7]]
[[1,9],[8,5]]
[[[[1,2],[3,4]],[[5,6],[7,8]]],9]
[[[9,[3,8]],[[0,9],6]],[[[3,7],[4,9]],3]]
[[[[1,3],[5,3]],[[1,3],[8,7]]],[[[4,9],[6,9]],[[8,2],[7,3]]]]";

    public Day18()
        : base(18, AdventDayImplementation.Build(AdventDataSource.FromRaw(TestInput), Parse, PartOne))
    { }

    private static SnailFishNumber[] Parse(string input) => input.Split(Environment.NewLine).Select(SnailFishNumber.Parse).ToArray();

    private static string PartOne(SnailFishNumber[] data)
    {
        var number = data[0];

        foreach (var snailFishNumber in data.Skip(1))
        {
            number = SnailFishNumber.Add(number, snailFishNumber);
            var reduceResult = number.Reduce();

            Debug.Assert(reduceResult is CompletedReduceOperation);
        }

        return number.CalculateMagnitude().ToString();
    }

    private static string PartTwo(SnailFishNumber[] data) => data.ToString()!;
}