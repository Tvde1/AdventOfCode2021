using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.EmptyDay;

public class Day25 : AdventDay
{
    private const string InputFile = "Day25/day25.txt";

    private const string TestInput = @"";

    public Day25()
        : base(25)
    {
        AddPart(PartOne);
        AddPart(PartTwo);
    }

    public static AdventDayPart PartOne =>
        AdventDayPart.Build(
            InputFile,
            input => input,
            data => data);

    public static AdventDayPart PartTwo =>
        AdventDayPart.Build(
            InputFile,
            input => input,
            data => data);
}