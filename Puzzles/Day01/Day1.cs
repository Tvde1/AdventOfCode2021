using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day01;

using Part1Data = IEnumerable<int>;

public class Day1 : AdventDay<Part1Data>
{
    private const string InputFile = "Day1/day1.txt";

    public Day1() : base(1, AdventDataSource.FromFile(InputFile), Parse, PartOne, PartTwo)
    {    }

    public static Part1Data Parse(string s) => s.Split(Environment.NewLine).Select(int.Parse);

    public static string PartOne(Part1Data data) =>
        data.Pairs()
            .Count(x => x.Item1 < x.Item2)
            .ToString();

    public static string PartTwo(Part1Data data) =>
        data.Triplets()
            .Select(x => x.Item1 + x.Item2 + x.Item3)
            .Pairs()
            .Count(x => x.Item1 < x.Item2)
            .ToString();
}