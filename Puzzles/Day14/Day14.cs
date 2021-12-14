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
                    data = SimulationSolution.IteratePolymerTemplate(data);
                }

                return data.Hash();
            });

    public static AdventAssignment PartTwo =>
        AdventAssignment.Build(
            InputFile,
            PolymerTemplate.Parse,
            data => PairIteratorSolution.CalculatePolymerHashFast(data, 40));

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

    private static class SimulationSolution
    {

        public static PolymerTemplate IteratePolymerTemplate(PolymerTemplate template)
        {
            var pairs = template.Polymer.Pairs();

            var newPolymer =
                string.Join(string.Empty, pairs.Select(x => x.Item1.ToString() + template.PolymerInstructions[x])) +
                template.Polymer[^1];

            return template with { Polymer = newPolymer };
        }
    }

    private static class PairIteratorSolution
    {
        public static long CalculatePolymerHashFast(PolymerTemplate template, int iterations)
        {
            var startPairs = template.Polymer.Pairs().ToList();

            var pairCounts = startPairs.GroupBy(x => x).ToDictionary(x => x.Key, x => (long)x.Count());

            for (var i = 1; i <= iterations; i++)
            {
                pairCounts = IncrementPairs(pairCounts, template.PolymerInstructions);
            }

            pairCounts[startPairs[0]] += 1;
            pairCounts[startPairs[^1]] += 1;

            var charCounts = new Dictionary<char, long>();

            foreach (var ((char1, char2), index) in pairCounts)
            {
                charCounts.UpdateOrAdd(char1, i => i + index, index);
                charCounts.UpdateOrAdd(char2, i => i + index, index);
            }

            return charCounts.Values.Max() / 2 - charCounts.Values.Min() / 2;
        }

        private static Dictionary<(char, char), long> IncrementPairs(Dictionary<(char, char), long> pairCount,
            IReadOnlyDictionary<(char, char), char> instructions)
        {
            var returnDict = instructions.Keys.Distinct().ToDictionary(x => x, _ => 0L);
            foreach (var (currentPair, currentPairOccurrenceCount) in pairCount.Where(x => x.Value != 0))
            {
                var (newPair1, newPair2) = CalculatePolymerStep(currentPair, instructions);

                returnDict[newPair1] += currentPairOccurrenceCount;
                returnDict[newPair2] += currentPairOccurrenceCount;
            }

            return returnDict;
        }

        private static ((char, char), (char, char)) CalculatePolymerStep((char, char) pair,
            IReadOnlyDictionary<(char, char), char> instructions)
        {
            var polymerInBetween = instructions[pair];
            return ((pair.Item1, polymerInBetween), (polymerInBetween, pair.Item2));
        }
    }
}