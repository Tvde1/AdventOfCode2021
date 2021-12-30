using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Puzzles._2021.Day16;

namespace AdventOfCode.Puzzles._2020.Day09;

public class Day9 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"35
20
15
25
47
40
62
55
65
95
102
117
150
182
127
219
299
277
309
576");

    public Day9()
        : base(AdventDayImplementation.Build(RealInput, Parse, PartOne, PartTwo))
    { }

    private static long[] Parse(string input) => input.Split(Environment.NewLine).Select(long.Parse).ToArray();

    private const int PreambleSize = 25;

    private static string PartOne(long[] data)
    {
        return FindInvalidNumber(data, PreambleSize)!.Value.Number.ToString();
    }

    private static string PartTwo(long[] data)
    {
        var invalidNumber = FindInvalidNumber(data, PreambleSize)!.Value;

        for (var i = 0; i < invalidNumber.Index; i++)
        {
            var sumNumbers = new List<long> {data[i]};
            var total = data[i];

            var next = i + 1;
            while (true)
            {
                var current = data[next];

                total += current;
                sumNumbers.Add(current);

                if (total == invalidNumber.Number)
                {
                    return (sumNumbers.Min() + sumNumbers.Max()).ToString();
                }

                if (total > invalidNumber.Number)
                {
                    break;
                }

                next++;
            }
        }

        return "not found";
    }

    private static (int Index, long Number)? FindInvalidNumber(long[] data, int preambleSize)
    {
        for (var i = preambleSize; i < data.Length; i++)
        {
            var numStart = i - preambleSize;
            var numEnd = i;
            var numbersToTakeIntoAccount = data[numStart..numEnd];

            var num = data[i];

            var doesAdd = numbersToTakeIntoAccount.Any(num1 =>
                numbersToTakeIntoAccount.Where(num2 => num2 != num1).Select(num2 => num1 + num2).Any(sum => sum == num));

            if (!doesAdd)
            {
                return (i, num);
            }
        }

        return null;
    }
}