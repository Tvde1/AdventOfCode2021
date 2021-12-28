using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Puzzles._2020.Day01;

public class Day1 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"");

    public Day1()
        : base(AdventDayImplementation.Build(TestInput, Parse, PartOne))
    { }

    private static string Parse(string input) => input;

    private static string PartOne(string data) => data;
}