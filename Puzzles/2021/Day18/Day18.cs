using System;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles._2021.Day18;

public class Day18 : AdventDay
{
    private const string InputFile = "Day18/day18.txt";

    private const string TestInputWorks = @"[[[[4,3],4],4],[7,[[8,4],9]]]
[1,1]";

    private const string TestInput = @"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]";

    private const string TestInput2 = @"[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]";

    public Day18()
        : base(18, AdventDayImplementation.Build(AdventDataSource.FromFile(InputFile), Parse, PartOne, PartTwo))
    { }

    private static SnailFishNumber[] Parse(string input) => input.Split(Environment.NewLine).Select(SnailFishNumber.Parse).ToArray();

    private static string PartOne(SnailFishNumber[] data) => 
        data.Aggregate((total, newNum) => SnailFishNumber.Add(total, newNum))
            .CalculateMagnitude()
            .ToString();

    private static string PartTwo(SnailFishNumber[] data)
    {
        var highestMagnitude = 0;

        foreach (var number1 in data)
        {
            foreach (var number2 in data)
            {
                if (number1 == number2)
                {
                    continue;
                }

                var product = SnailFishNumber.Add(number1, number2);

                var magnitude = product.CalculateMagnitude();
                if (magnitude > highestMagnitude)
                {
                    highestMagnitude = magnitude;
                }
            }
        }

        return highestMagnitude.ToString();
    }
}