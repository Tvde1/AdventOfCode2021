using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Numerics;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles._2020.Day03;

public class Day3 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"..##.......
#...#...#..
.#....#..#.
..#.#...#.#
.#...##..#.
..#.##.....
.#.#.#....#
.#........#
#.##...#...
#...##....#
.#..#...#.#");

    public Day3()
        : base(AdventDayImplementation.Build(RealInput, Parse, PartOne, PartTwo))
    { }

    private static bool[,] Parse(string input) => input.Split(Environment.NewLine).Select(x => x.Select(y => y == '#')).ToTwoDimensionalArray();

    private static string PartOne(bool[,] data)
    {
        var translation = new Vector2D(3, 1);
        var treeCount = TreeCount(data, translation);

        return treeCount.ToString();
    }

    private static string PartTwo(bool[,] data)
    {
        var translations = new Vector2D[]
        {
            new(1, 1),
            new(3, 1),
            new(5, 1),
            new(7, 1),
            new(1, 2),
        };

        var treeCounts = translations.Select(x => (long) TreeCount(data, x));

        var multiplied = treeCounts.Aggregate((a, b) => a * b);

        return multiplied.ToString();
    }

    private static int TreeCount(bool[,] data, Vector2D translation)
    {
        var point = new Point2D(0, 0);

        var yEnd = data.GetLength(0);
        var xEnd = data.GetLength(1);

        var treeCount = 0;

        while (true)
        {
            if (point.Y >= yEnd)
            {
                break;
            }

            if (point.X >= xEnd)
            {
                point = point with {X = point.X - xEnd};
            }

            if (data.GetPoint(point))
            {
                treeCount++;
            }

            point = point.Translate(translation);
        }

        return treeCount;
    }
}