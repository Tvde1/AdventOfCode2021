using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Puzzles._2020.Day7;

public class Day7 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags.");

    public Day7()
        : base(AdventDayImplementation.Build(TestInput, Parse, PartOne))
    { }

    private readonly record struct Bag(string BagColour, string BagStyle)
    {
        public static Bag Parse(string input)
        {
            var sp = input.Split(StringConstants.Space);
            return new Bag(sp[1], sp[0]);
        }
    }

    private readonly record struct BagRule(Bag Subject, (Bag Bag, int Count)[] Contents)
    {
        public static BagRule Parse(string input)
        {
            var sp = input[..^1].Split("contain");
            var subject = Bag.Parse(sp[0].Split("bags")[0]);

            var contents = sp[1] switch
            {
                "no other bags" => Array.Empty<(Bag, int)>(),
                _ => sp[1].Split(", ").Select(x =>
                {
                    var sspl = x.Split(" ");

                    var bag = Bag.Parse(string.Join(StringConstants.Space, sspl[1..^1]));

                    return (bag, int.Parse(sspl[0]));
                }).ToArray(),
            };

            return new BagRule(subject, contents);
        }
    }

    private static List<BagRule> Parse(string input) => input.Split(Environment.NewLine).Select(BagRule.Parse).ToList();

    private static string PartOne(string data) => data;
}