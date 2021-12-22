using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day25;

public class Day25 : AdventDay
{
    private static AdventDataSource RealInput = AdventDataSource.FromFile("Day25/day25.txt");

    private static AdventDataSource TestInput = AdventDataSource.FromRaw(@"");

    public Day25()
        : base(25, AdventDayImplementation.Build(TestInput, Parse))
    { }

    private static string Parse(string input) => input;

    private static string PartOne(string data) => data;

    private static string PartTwo(string data) => data;
}