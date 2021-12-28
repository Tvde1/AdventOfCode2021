using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Puzzles._2020.Day02;

public class Day2 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"1-3 a: abcde
1-3 b: cdefg
2-9 c: ccccccccc");

    public Day2()
        : base(AdventDayImplementation.Build(RealInput, Parse, PartOne, PartTwo))
    { }

    private readonly record struct PasswordWithPolicy(Range CharacterRange, char Character, string Password)
    {
        public static PasswordWithPolicy Parse(string input)
        {
            var spl = input.Split(":");

            var range = spl[0].Split(StringConstants.Space)[0].Split('-');

            var r = int.Parse(range[0])..int.Parse(range[1]);

            return new PasswordWithPolicy(r, spl[0][^1], spl[1]);
        }
    }

    private static List<PasswordWithPolicy> Parse(string input) => input.Split(Environment.NewLine).Select(PasswordWithPolicy.Parse).ToList();

    private static string PartOne(List<PasswordWithPolicy> data)
    {
        var count = 0;
        foreach (var passwordWithPolicy in data)
        {
            var charCount = passwordWithPolicy.Password.Count(x => x == passwordWithPolicy.Character);

            if (passwordWithPolicy.CharacterRange.Contains(charCount))
            {
                count++;
            }
        }

        return count.ToString();
    }

    private static string PartTwo(List<PasswordWithPolicy> data)
    {
        var count = 0;
        foreach (var passwordWithPolicy in data)
        {
            if (passwordWithPolicy.Password[passwordWithPolicy.CharacterRange.Start] == passwordWithPolicy.Character)
            {
                if (passwordWithPolicy.Password[passwordWithPolicy.CharacterRange.End] != passwordWithPolicy.Character)
                {
                    count++;
                }
            }
            else
            {
                if (passwordWithPolicy.Password[passwordWithPolicy.CharacterRange.End] == passwordWithPolicy.Character)
                {
                    count++;
                }
            }
        }

        return count.ToString();
    }
}