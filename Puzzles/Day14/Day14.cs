using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Puzzles.Day14;

public class Day14 : AdventDayBase
{
    private const string InputFile = "Day14/day14.txt";

    private const string TestInput = @"NNCB

CH -> B
HH -> N
CB -> H
NH -> C
HB -> C
HC -> B
HN -> C
NN -> C
BH -> H
NC -> B
NB -> B
BN -> B
BB -> N
BC -> B
CC -> N
CN -> C";

    public Day14()
        : base(14)
    {
        AddPart(PartOne);
        AddPart(PartTwo);
    }

    public static AdventAssignment PartOne =>
        AdventAssignment.Build(
            InputFile,
            _ => PolymerTemplate.Parse(TestInput),
            data =>
            {
                for (var i = 1; i <= 10; i++)
                {
                    data = SimpleSolution.IteratePolymerTemplate(data);
                }

                return data.Hash();
            });

    public static AdventAssignment PartTwo =>
        AdventAssignment.Build(
            InputFile,
            _ => PolymerTemplate.Parse(TestInput),
            data => IteratorSolution.CalculatePolymerHashFast(data, 15));

    public record PolymerTemplate(string Polymer, IReadOnlyDictionary<(char, char), char> PolymerInstructions)
    {
        public static PolymerTemplate Parse(string s)
        {
            var lines = s.Split(Environment.NewLine);

            var polymer = lines[0];

            var instructions = lines
                .Skip(2)
                .Select(instructionLine => instructionLine.Split(" -> "))
                .ToDictionary(split => (split[0][0], split[0][1]), split => split[1][0]);

            return new PolymerTemplate(polymer, instructions);
        }

        public int Hash()
        {
            checked
            {
                return Polymer.MostCommonCount() - Polymer.LeastCommonCount();
            }
        }
    }


    public static class SimpleSolution
    {
        public static PolymerTemplate IteratePolymerTemplate(PolymerTemplate template)
        {
            var pairs = template.Polymer.Pairs();

            var newPolymer = string.Join(string.Empty, pairs.Select(x => x.Item1.ToString() + template.PolymerInstructions[x])) + template.Polymer[^1];

            return template with { Polymer = newPolymer };
        }
    }

    public static class IteratorSolution
    {
        public static long CalculatePolymerHashFast(PolymerTemplate template, int iterations)
        {
            var startPairs = template.Polymer.Pairs();

            var charCounts = template.PolymerInstructions.Values.Distinct().ToDictionary(x => x, _ => 0L);

            foreach (var pair in startPairs)
            {
                foreach (var polymerChar in CalculateIterations(pair, template.PolymerInstructions, iterations))
                {
                    charCounts[polymerChar] += 1;
                }
            }

            charCounts[template.Polymer[^1]] += 1;

            return charCounts.Values.Max() - charCounts.Values.Min();
        }

        public static IEnumerable<char> CalculateIterations((char, char) pair, IReadOnlyDictionary<(char, char), char> instructions, int iterations)
        {
            if (iterations == 0)
            {
                yield return pair.Item1;
                yield break;
            }

            var (subPair1, subPair2) = CalculatePolymerStep(pair, instructions);
            var it1 = CalculateIterations(subPair1, instructions, iterations - 1);
            var it2 = CalculateIterations(subPair2, instructions, iterations - 1);

            foreach (var subPair1Chars in it1)
            {
                yield return subPair1Chars;
            }

            foreach (var subPair2Chars in it2)
            {
                yield return subPair2Chars;
            }
        }

        public static ((char, char), (char, char)) CalculatePolymerStep((char, char) pair, IReadOnlyDictionary<(char, char), char> instructions)
        {
            var polymerInBetween = instructions[pair];
            return ((pair.Item1, polymerInBetween), (polymerInBetween, pair.Item2));
        }
    }
}