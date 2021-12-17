using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day08;

public class Day8 : AdventDay
{
    private const string InputFile = "Day8/day8.txt";

    private const string TestInput =
        @"be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe
edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc
fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg
fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb
aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea
fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb
dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe
bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef
egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb
gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce";

    public Day8()
        : base(8)
    {
        AddPart(PartOne);
        AddPart(PartTwo);
    }

    // Test: 26
    // 288
    public static AdventDayPart PartOne =>
        AdventDayPart.Build(
            InputFile,
            input => input.Split(Environment.NewLine).Select(SegmentData.Parse),
            data => data.Sum(x => x.Output.Count(a => a is {Length: 2 or 3 or 4 or 7})));

    public static AdventDayPart PartTwo =>
        AdventDayPart.Build(
            InputFile,
            input => input.Split(Environment.NewLine).Select(SegmentData.Parse),
            data => data.Sum(segmentData =>
                CalculateNumber(segmentData.Output, CalculateConnections(segmentData.Signals)))
        );

    private static IReadOnlyDictionary<string, int> CalculateConnections(string[] signals)
    {
        // Known
        var one = signals.Single(x => x is {Length: 2});
        var four = signals.Single(x => x is {Length: 4});
        var seven = signals.Single(x => x is {Length: 3});
        var eight = signals.Single(x => x is {Length: 7});

        // Figured out
        var three = signals.Single(x => x is {Length: 5} && ContainsAllChars(x, one));
        var nine = signals.Single(x => x is {Length: 6} && ContainsAllChars(x, four));
        var zero = signals.Except(nine.Enumerate()).Single(x => x is {Length: 6} && ContainsAllChars(x, one));
        var six = signals.Except(new[] {nine, zero}).Single(x => x is {Length: 6});

        var two = signals.Except(new[] {three}).Single(x =>
        {
            if (x is {Length: 5})
            {
                var enumerable = six.Except(x).ToList();
                var b = enumerable.Count == 2;
                return b;
            }

            return false;
        });
        var five = signals.Except(new[] {three, two}).Single(x => x is {Length: 5} && six.Except(x).Count() == 1);

        var mapping = new Dictionary<string, int>
        {
            {one, 1},
            {two, 2},
            {three, 3},
            {four, 4},
            {five, 5},
            {six, 6},
            {seven, 7},
            {eight, 8},
            {nine, 9},
            {zero, 0}
        };

        Debug.Assert(mapping.Keys.Count == 10);

        return mapping;
    }

    private static bool ContainsAllChars(string input, string sub)
    {
        return sub.ToCharArray().All(input.Contains);
    }

    private static int CalculateNumber(IEnumerable<string> inputs, IReadOnlyDictionary<string, int> mapping)
    {
        return inputs.Select((t, i) => mapping[t] * Convert.ToInt32(Math.Pow(10, 3 - i))).Sum();
    }

    private readonly record struct SegmentData(string[] Signals, string[] Output)
    {
        public static SegmentData Parse(string input)
        {
            var s = input.Split(" | ");

            var signals = s[0].Split(StringConstants.Space, StringSplitOptions.TrimEntries)
                .Select(x => x.ToAlphabeticalOrder()).ToArray();

            var output = s[1].Split(StringConstants.Space, StringSplitOptions.TrimEntries)
                .Select(x => x.ToAlphabeticalOrder()).ToArray();

            return new SegmentData(signals, output);
        }
    }
}