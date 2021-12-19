using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day25;

public class Day25 : AdventDay
{
    private const string InputFile = "Day25/day25.txt";

    private const string TestInput = @"";

    public Day25()
        : base(25, AdventDayImplementation.Build(AdventDataSource.FromRaw(TestInput), Parse))
    { }

    private static string Parse(string input) => input;

    private static string PartOne(string data) => data;

    private static string PartTwo(string data) => data;
}