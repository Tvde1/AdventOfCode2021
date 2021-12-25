using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Puzzles.Day25;

public class Day25 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.FromFile("Day25/day25.txt");

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"v...>>.vv>
.vv>>.vv..
>>.>v>...v
>>v>>.>.v.
v>v.vv.v..
>.>>..v...
.vv..>.>v.
v.v..>>v.v
....v..v.>");

    private static readonly AdventDataSource TestInput2 = AdventDataSource.FromRaw(@">>>.
..v.
....");

    public Day25()
        : base(25, AdventDayImplementation.Build(RealInput, Parse, PartOne))
    { }

    private readonly record struct SeaCucumberCell(bool IsEmpty, bool IsEast, bool IsSouth)
    {
        public static SeaCucumberCell Parse(char input) => input switch
        {
            '.' => new(true, false, false),
            '>' => new(false, true, false),
            'v' => new(false, false, true),
            _ => throw new ArgumentException(),
        };
    }

    private static SeaCucumberCell[,] Parse(string input) => input.Split(Environment.NewLine).Select(x => x.Select(SeaCucumberCell.Parse)).ToTwoDimensionalArray().Flip();

    private static string PartOne(SeaCucumberCell[,] data)
    {
        int iteration = 0;

        var width = data.GetLength(1);
        var height = data.GetLength(0);

        var hasMoved = true;
        while (hasMoved)
        {
            hasMoved = false;

            var newState = new SeaCucumberCell[height, width];
            newState.Paste(data);

            iteration++;

            //Console.WriteLine($"Iteration: {iteration++}");
            //Console.WriteLine(data.Flip().Render(x => x switch
            //{
            //    (true, false, false) => '.',
            //    (false, true, false) => '>',
            //    (false, false, true) => 'v',
            //    _ => throw new ArgumentException(),
            //}));
            //Console.WriteLine();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var cucumber = data[y, x];

                    if (!cucumber.IsEast)
                    {
                        continue;
                    }

                    var nextSpot = x + 1;
                    if (nextSpot == width) nextSpot = 0;

                    if (data[y, nextSpot].IsEmpty)
                    {
                        newState[y, x] = new SeaCucumberCell(true, false, false);
                        newState[y, nextSpot] = cucumber;
                        hasMoved = true;
                    }
                }
            }

            data = newState;
            newState = new SeaCucumberCell[height, width];
            newState.Paste(data);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var cucumber = data[y, x];

                    if (!cucumber.IsSouth)
                    {
                        continue;
                    }

                    var nextSpot = y + 1;
                    if (nextSpot == height) nextSpot = 0;

                    if (data[nextSpot, x].IsEmpty)
                    {
                        newState[y, x] = new SeaCucumberCell(true, false, false);
                        newState[nextSpot, x] = cucumber;
                        hasMoved = true;
                    }
                }
            }

            data = newState;
        }

        return iteration.ToString();
    }
}