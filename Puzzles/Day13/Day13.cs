using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Monads;

namespace AdventOfCode.Puzzles.Day13;

public class Day13 : AdventDayBase
{
    private const string InputFile = "Day13/day13.txt";

    private const string TestInput = @"";

    public Day13()
        : base(13)
    {
        AddPart(PartOne);
        AddPart(PartTwo);
    }

    public static AdventAssignment PartOne =>
        AdventAssignment.Build(
            InputFile,
            input => TestInput,
            data => data);

    public static AdventAssignment PartTwo =>
        AdventAssignment.Build(
            InputFile,
            input => TestInput,
            data => data);
}