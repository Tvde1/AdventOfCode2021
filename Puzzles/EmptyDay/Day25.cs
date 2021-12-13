using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Monads;

namespace AdventOfCode.Puzzles.Day25;

public class Day25 : AdventDayBase
{
    private const string InputFile = "Day25/day25.txt";

    private const string TestInput = @"";

    public Day25()
        : base(25)
    {
        AddPart(PartOne);
        AddPart(PartTwo);
    }

    public static AdventAssignment PartOne =>
        AdventAssignment.Build(
            InputFile,
            input => input,
            data => data);

    public static AdventAssignment PartTwo =>
        AdventAssignment.Build(
            InputFile,
            input => input,
            data => data);
}