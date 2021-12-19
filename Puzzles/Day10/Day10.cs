using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Monads;

namespace AdventOfCode.Puzzles.Day10;

public class Day10 : AdventDay
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
        : base(10, AdventDayImplementation.Build(AdventDataSource.FromFile(InputFile), Parse, PartOne, PartTwo))
    { }

    private static string[] Parse(string input) => input.Split(Environment.NewLine);

    private static string PartOne(string[] data) => 
        data.AsParallel()
            .Select(GetNavigationSubsystemError)
            .WhereNotNull()
            .WhereLeft()
            .Select(x => UnexpectedTokenErrorScores[x.found])
            .Sum()
            .ToString();

    private static string PartTwo(string[] data) =>
        data.AsParallel()
            .Select(GetNavigationSubsystemError)
            .WhereNotNull()
            .WhereRight()
            .Select(GetAutocompleteScore)
            .OrderBy(x => x)
            .Middle()
            .ToString();

    private static readonly Dictionary<char, char> Opposites = new()
    {
        {'(', ')'},
        {'[', ']'},
        {'{', '}'},
        {'<', '>'},
    };

    private static readonly Dictionary<char, int> UnexpectedTokenErrorScores = new()
    {
        {')', 3},
        {']', 57},
        {'}', 1197},
        {'>', 25137},
    };

    private static readonly Dictionary<char, int> IncompleteTokenErrorScores = new()
    {
        {')', 1},
        {']', 2},
        {'}', 3},
        {'>', 4},
    };

    private static Either<(char expected, char found), char[]>? GetNavigationSubsystemError(string input)
    {
        var stack = new Stack<char>();

        foreach (var character in input)
        {
            if (character is '(' or '[' or '{' or '<')
            {
                stack.Push(Opposites[character]);
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

        return stack.Any() ? stack.ToArray() : (Either<(char expected, char found), char[]>?) null;
    }

    private static long GetAutocompleteScore(char[] missingCharacters)
    {
        var score = 0L;

        foreach (var character in missingCharacters)
        {
            checked
            {
                score *= 5;
                score += IncompleteTokenErrorScores[character];
            }
        }

        return score;
    }
}