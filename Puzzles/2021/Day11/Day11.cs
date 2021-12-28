using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles._2021.Day11;

public class Day11 : AdventDay
{

    private const string TestInput = @"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526";

    public Day11()
        : base(AdventDayImplementation.Build(AdventDataSource.ForThisDay(), Parse, PartOne, PartTwo))
    { }

    public static int[,] Parse(string input) =>
        input.Split(Environment.NewLine)
            .Select(x => x.Select(y => int.Parse(y.ToString())))
            .ToTwoDimensionalArray();

    public static string PartOne(int[,] data)
    {
        var totalFlashCount = 0;
        for (var i = 1; i <= 100; i++)
        {
            data = Step(data, out var amountFlashed);
            totalFlashCount += amountFlashed;
        }

        return totalFlashCount.ToString();
    }

    public static string PartTwo(int[,] data)
    {
        var i = 1;
        for (; ; i++)
        {
            data = Step(data, out _);
            if (data.Flatten().All(x => x == 0))
            {
                break;
            }
        }

        return i.ToString();
    }

    private static int[,] Step(int[,] input, out int amountFlashed)
    {
        var width = input.GetLength(0);
        var height = input.GetLength(1);

        var pointFactory = Enumerable.Range(0, width)
            .SelectMany(currentWidth =>
                Enumerable.Range(0, height)
                    .Select(currentHeight => new Point2D(currentWidth, currentHeight))).ToList();
        var pointsToVisit = new Queue<Point2D>(pointFactory);

        Debug.Assert(pointsToVisit.Count == 100);

        while (pointsToVisit.TryDequeue(out var currentPoint2D))
        {
            var current = input[currentPoint2D.X, currentPoint2D.Y];

            if (current != -1)
            {
                current++;
            }

            if (current > 9)
            {
                current = -1;

                var u = new Point2D(currentPoint2D.X, currentPoint2D.Y + 1);
                var ul = new Point2D(currentPoint2D.X - 1, currentPoint2D.Y + 1);
                var ur = new Point2D(currentPoint2D.X + 1, currentPoint2D.Y + 1);
                var l = new Point2D(currentPoint2D.X - 1, currentPoint2D.Y);
                var r = new Point2D(currentPoint2D.X + 1, currentPoint2D.Y);
                var d = new Point2D(currentPoint2D.X, currentPoint2D.Y - 1);
                var dl = new Point2D(currentPoint2D.X - 1, currentPoint2D.Y - 1);
                var dr = new Point2D(currentPoint2D.X + 1, currentPoint2D.Y - 1);

                var neighbors = new[]
                {
                    u,
                    ul,
                    ur,
                    l,
                    r,
                    dl,
                    dr,
                    d
                };

                foreach (var n in neighbors)
                {
                    if (input.TryGet(n.X, n.Y) is not null and not -1)
                    {
                        pointsToVisit.Enqueue(n);
                    }
                }
            }

            input[currentPoint2D.X, currentPoint2D.Y] = current;
        }

        amountFlashed = 0;
        foreach (var p in pointFactory)
        {
            var val = input[p.X, p.Y];

            if (val == -1)
            {
                amountFlashed++;
                input[p.X, p.Y] = 0;
            }
        }

        return input;
    }
}