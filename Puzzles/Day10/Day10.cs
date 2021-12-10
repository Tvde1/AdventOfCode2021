using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Monads;

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
        AddPart(PartTwo);
    }

    public static AdventAssignment PartOne =>
        AdventAssignment.Build(
            InputFile,
            input => input.Split(Environment.NewLine),
            data => data
            // .AsParallel()
            .Select(GetNavigationSubsystemError)
            .WhereNotNull()
            .WhereLeft()
            .Select(x => _unexpectedTokenErrorScores[x.found])
            .Sum());

    public static AdventAssignment PartTwo =>
        AdventAssignment.Build(
            InputFile,
            input => input.Split(Environment.NewLine),
            data => data
            // .AsParallel()
            .Select(GetNavigationSubsystemError)
            .WhereNotNull()
            .WhereRight()
            .Select(GetAutocompleteScore)
            .OrderBy(x => x)
            .Middle());

    private static readonly Dictionary<char, char> _opposites = new()
    {
        { '(', ')' },
        { '[', ']' },
        { '{', '}' },
        { '<', '>' },
    };

    private static readonly Dictionary<char, int> _unexpectedTokenErrorScores = new()
    {
        { ')', 3 },
        { ']', 57 },
        { '}', 1197 },
        { '>', 25137 },
    };

    private static readonly Dictionary<char, int> _incompleteTokenErrorScores = new()
    {
        { ')', 1 },
        { ']', 2 },
        { '}', 3 },
        { '>', 4 },
    };

    public static Either<(char expected, char found), char[]>? GetNavigationSubsystemError(string input)
    {
        var stack = new Stack<char>();

        foreach (var character in input)
        {
            if (character is '(' or '[' or '{' or '<')
            {
                stack.Push(_opposites[character]);
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

        if (stack.Any())
        {
            return stack.ToArray();
        }

        return null;
    }

    private static long GetAutocompleteScore(char[] missingCharacters)
    {
        var score = 0L;

        foreach (var character in missingCharacters)
        {
            checked
            {
                score *= 5;
                score += _incompleteTokenErrorScores[character];
            }
        }

        return score;
    }
}