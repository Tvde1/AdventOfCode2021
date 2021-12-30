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

    private static readonly AdventDataSource TestInput2 = AdventDataSource.FromRaw(@"shiny gold bags contain 2 dark red bags.
dark red bags contain 2 dark orange bags.
dark orange bags contain 2 dark yellow bags.
dark yellow bags contain 2 dark green bags.
dark green bags contain 2 dark blue bags.
dark blue bags contain 2 dark violet bags.
dark violet bags contain no other bags.");

    public Day7()
        : base(AdventDayImplementation.Build(TestInput2, Parse, PartOne, PartTwo))
    { }

    private readonly record struct Bag(string BagColour, string BagStyle)
    {
        public override string ToString()
        {
            return $"{BagStyle} {BagColour}";
        }

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
            var sp = input[..^1].Split("contain ");
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

    private static Dictionary<Bag, BagRule> Parse(string input) => input.Split(Environment.NewLine).Select(BagRule.Parse).ToDictionary(x => x.Subject, x=>x);

    private static string PartOne(Dictionary<Bag, BagRule> data)
    {
        var target = new Bag("gold", "shiny");

        var containers = GetPossibleContainers(data, target, new HashSet<Bag>()).ToList();

        Console.WriteLine(string.Join(", ", containers));

        return GetPossibleContainers(data, target, new HashSet<Bag>()).Count().ToString();
    }

    private static string PartTwo(Dictionary<Bag, BagRule> data)
    {
        var bag = new Bag("gold", "shiny");

        var childrenCount = GetChildrenCount(data, bag) - 1;

        return childrenCount.ToString();
    }


    private static IEnumerable<Bag> GetPossibleContainers(Dictionary<Bag, BagRule> data, Bag toBeContained, HashSet<Bag> visitedBags)
    {
        var containers = data.Where(x => x.Value.Contents.Any(y => y.Bag == toBeContained));

        foreach (var container in containers)
        {
            if (!visitedBags.Add(container.Key))
            {
                continue;
            }

            yield return container.Key;

            foreach (var containerPossibility in GetPossibleContainers(data, container.Key, visitedBags))
            {
                yield return containerPossibility;
            }
        }
    }

    private static int GetChildrenCount(IReadOnlyDictionary<Bag, BagRule> rules, Bag bag)
    {
        return 1 + rules[bag].Contents.Sum(subBag => subBag.Count * GetChildrenCount(rules, subBag.Bag));
    }
}