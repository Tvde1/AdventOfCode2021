using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day10;

public class Day10 : AdventDayBase
{
    private const string InputFile = "Day10/day10.txt";

    private const string TestInput = @"[({(<(())[]>[[{[]{<()<>>
[(()[<>])]({[<{<<[]>>(
{([(<{}[<>[]}>{[]{[(<()>
(((({<>}<{<{<>}{[]{[]{}
[[<[([]))<([[{}[[()]]]
[{[{({}]{}}([{[{{{}}([]
{<[[]]>}<{[{[{[]{()[[[]
[<(<(<(<{}))><([]([]()
<{([([[(<>()){}]>(<<{{
<{([{{}}[<[[[<>{}]]]>[]]";

    public Day10()
        : base(10)
    {
        AddPart(PartOne);
        // AddPart(PartTwo);
    }

    public static AdventAssignment PartOne =>
        AdventAssignment.Build(
            InputFile,
            input => TestInput.Split(Environment.NewLine),
            data => data.Select(GetNavigationSubsystemError).WhereNotNull().Select(x => _errorScores[x.found]).Sum());

    //public static AdventAssignment PartTwo =>
    //    AdventAssignment.Build(
    //        InputFile,
    //        input => input.Split(Environment.NewLine).Select(x => x.Select(c => int.Parse(c.ToString())))
    //            .ToTwoDimensionalArray(),
    //        data => CalculateLowestPointsParallel(data).Select(x => CalculateBasinSize(data, x))
    //            .OrderByDescending(x => x).Take(3).Aggregate(1, (a, b) => a * b));

    public static (char expected, char found)? GetNavigationSubsystemError(string input)
    {
        var stack = new Stack<char>();

        foreach (var character in input)
        {
            if (character is '(' or '[' or '{' or '<')
            {
                stack.Push(character);
                continue;
            }

            if (!stack.TryPop(out var expected))
            {
                break;
            }

            if (expected != character)
            {
                return (expected, character);
            }
        }

        return null;
    }

    private static readonly Dictionary<char, int> _errorScores = new()
    {
        { ')', 3 },
        { ']', 57 },
        { '}', 1197 },
        { '>', 25137 },
    };
}