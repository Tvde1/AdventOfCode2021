using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Monads;

namespace AdventOfCode.Puzzles.Day11;

public class Day11 : AdventDayBase
{
    private const string InputFile = "Day11/day11.txt";

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
        : base(11)
    {
        AddPart(PartOne);
        AddPart(PartTwo);
    }

    public static AdventAssignment PartOne =>
        AdventAssignment.Build(
            InputFile,
            input => TestInput.Split(Environment.NewLine).Select(x => x.Select(y => int.Parse(y.ToString()))).ToTwoDimensionalArray(),
            data =>
            {
                var totalFlashCount = 0;
                for (var i = 1; i <= 100; i++)
                {
                    data = Step(data, out var amountFlashed);
                    totalFlashCount += amountFlashed;
                    PrintBoard(data, i);
                }
                return totalFlashCount;
            });

    public static AdventAssignment PartTwo =>
        AdventAssignment.Build(
            InputFile,
            input => input.Split(Environment.NewLine),
            data => data);

    private static void PrintBoard(int[,] board, int step)
    {
        Console.WriteLine($"After step {step}:");
        var width = board.GetLength(0);
        var height = board.GetLength(1);

        for (int currentHeight = 0; currentHeight < height; currentHeight++)
        {
            for (int currentWidth = 0; currentWidth < width; currentWidth++)
            {
                Console.Write(board[currentWidth, currentHeight]);
            }
            Console.Write(Environment.NewLine);
        }
        Console.Write(Environment.NewLine);
    }

    private readonly record struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }

    private static int[,] Step(int[,] input, out int amountFlashed)
    {
        var width = input.GetLength(0);
        var height = input.GetLength(1);

        var pointFactory = Enumerable.Range(0, width)
            .SelectMany(currentWidth =>
                Enumerable.Range(0, height)
                    .Select(currentHeight => new Point(currentWidth, currentHeight)));
        var pointsToVisit = new Queue<Point>(pointFactory);

        Debug.Assert(pointsToVisit.Count == 100);

        while (pointsToVisit.TryDequeue(out var currentPoint))
        {
            var current = input[currentPoint.X, currentPoint.Y];

            if (current != -1)
            {
                current++;
            }

            if (current > 9)
            {
                current = -1;

                var u = new Point(currentPoint.X, currentPoint.Y + 1);
                var ul = new Point(currentPoint.X - 1, currentPoint.Y + 1);
                var ur = new Point(currentPoint.X + 1, currentPoint.Y + 1);
                var l = new Point(currentPoint.X - 1, currentPoint.Y);
                var r = new Point(currentPoint.X + 1, currentPoint.Y);
                var d = new Point(currentPoint.X, currentPoint.Y - 1);
                var dl = new Point(currentPoint.X - 1, currentPoint.Y + 1);
                var dr = new Point(currentPoint.X + 1, currentPoint.Y + 1);

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

            input[currentPoint.X, currentPoint.Y] = current;
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