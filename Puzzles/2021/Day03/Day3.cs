using System;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles._2021.Day03;

public class Day3 : AdventDay
{
    private const string InputFile = "Day03/day3.txt";

    private const string testInput = @"00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010";

    public Day3() 
        : base(3, AdventDayImplementation.Build(AdventDataSource.FromFile(InputFile), Parse, PartOne, PartTwo))
    { }


    public static string[] Parse(string input) => input.Split(Environment.NewLine);

    // 2743844
    public static string PartOne(string[] data)
    {
        var aggregate = new int[data[0].Length];

        foreach (var line in data)
            for (var index = 0; index < line.Length; index++)
                aggregate[index] += line[index] == '1' ? 1 : -1;

        var result = aggregate.Aggregate(0, (agg, cur) =>
        {
            agg <<= 1;
            if (cur > 0) agg |= 1;

            return agg;
        });

        var inverseResult = ~result & 0b1111_1111_1111;

        return (result * inverseResult).ToString();
    }

    // 6677951
    public static string PartTwo(string[] data)
    {
        var mostCommon = Convert.ToInt32(FindMostCommon(data, false), 2);
        var leastCommon = Convert.ToInt32(FindMostCommon(data, true), 2);

        return (mostCommon * leastCommon).ToString();
    }

    public static string FindMostCommon(string[] input, bool inverse, int indexToMatch = 0)
    {
        var grouped = input.ToLookup(x => x[indexToMatch])
            .OrderBy(x => x.Count())
            .ThenBy(x => x.Key);

        var set = (!inverse ? grouped.Last() : grouped.First()).ToArray();

        return set.Length == 1 ? set[0] : FindMostCommon(set, inverse, indexToMatch + 1);
    }
}