using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Puzzles._2020.Day01;

public class Day1 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"1721
979
366
299
675
1456");

    public Day1()
        : base(AdventDayImplementation.Build(RealInput, Parse, PartOne, PartTwo))
    { }

    private static List<int> Parse(string input) => input.Split(Environment.NewLine).Select(int.Parse).ToList();

    private static string PartOne(List<int> data)
    {
        foreach (var num1 in data)
        {
            foreach (var num2 in data.Where(num2 => num1 + num2 == 2020))
            {
                return (num1 * num2).ToString();
            }
        }

        return "No answer";
    }

    private static string PartTwo(List<int> data)
    {
        foreach (var num1 in data)
        {
            foreach (var num2 in data)
            {
                foreach (var num3 in data.Where(num3 => num1 + num2 + num3 == 2020))
                {
                    return (num1 * num2 * num3).ToString();
                }
            }
        }

        return "No answer";
    }
}