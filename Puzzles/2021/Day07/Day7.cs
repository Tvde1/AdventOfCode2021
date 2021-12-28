using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles._2021.Day07;

public class Day7 : AdventDay
{
    private const string InputFile = "Day07/day7.txt";

    private const string TestInput = "16,1,2,0,4,2,7,1,2,14";

    public Day7()
        : base(7, AdventDayImplementation.Build(AdventDataSource.FromFile(InputFile), Parse, PartOne, PartTwo))
    { }

    public static int[] Parse(string input) => input.Split(",").Select(int.Parse).ToArray();

    // 355764
    public static string PartOne(int[] data) => FindMedianCrabCost(data).ToString();

    // 99634572
    public static string PartTwo(int[] data) => FindBruteForceCrabCost(data).ToString();

    public static int FindMedianCrabCost(IEnumerable<int> horizontalPositions)
    {
        var sorted = horizontalPositions.OrderBy(x => x).ToList();
        var median = sorted[sorted.Count / 2];

        return CalculateCost(sorted, median);
    }

    public static int FindBruteForceCrabCost(int[] horizontalPositions)
    {
        var sorted = horizontalPositions.OrderBy(x => x).ToList();

        var range = sorted.Min()..sorted.Max();

        List<int> costs = new();
        foreach (var x in range) costs.Add(CalculateCostTwo(sorted, x));

        return costs.Min();
    }

    public static int CalculateCost(IEnumerable<int> horizontalPositions, int alignPosition)
    {
        return horizontalPositions.Sum(x => Math.Abs(alignPosition - x));
    }

    public static int CalculateCostTwo(IEnumerable<int> horizontalPositions, int alignPosition)
    {
        return horizontalPositions.Sum(x =>
        {
            var n = Math.Abs(alignPosition - x);
            return (int) (Math.Pow(n, 2) + n) / 2;
        });
    }
}