using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Puzzles._2020.Day05;

public class Day6 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"abc

a
b
c

ab
ac

a
a
a
a

b");

    public Day6()
        : base(AdventDayImplementation.Build(RealInput, Parse, PartOne, PartTwo))
    { }

    private static List<List<string>> Parse(string input)
    {
        var groups = new List<List<string>>();

        var groupAnswers = new List<string>();

        var lines = input.Split(Environment.NewLine);
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                groups.Add(groupAnswers);
                groupAnswers = new List<string>();
                continue;
            }

            groupAnswers.Add(line);
        }

        groups.Add(groupAnswers);
        return groups;
    }

    private static string PartOne(List<List<string>> data)
    {
        return data.Select(x =>
        {
            var selectMany = x.SelectMany(y => y);
            var distinct = selectMany.Distinct();
            var count = distinct.Count();
            return count;
        }).Sum().ToString();
    }

    private static string PartTwo(List<List<string>> data)
    {
        return data.Select(x =>
        {
            var selectMany = x.SelectMany(y => y);
            var distinct = selectMany.Distinct();
            var count = distinct.Count(c => x.All(a => a.Contains(c)));
            return count;
        }).Sum().ToString();
    }
}