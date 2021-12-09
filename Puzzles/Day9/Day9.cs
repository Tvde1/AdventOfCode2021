using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day9;

public class Day9 : AdventDayBase
{
    private const string InputFile = "Day9/day9.txt";

    private const string TestInput =
        @"2199943210
3987894921
9856789892
8767896789
9899965678";

    public Day9()
        : base(9)
    {
        AddPart(PartOne);
        AddPart(PartTwo);
    }

    public static AdventAssignment PartOne =>
        AdventAssignment.Build(
            InputFile,
            input => input.Split(Environment.NewLine).Select(x => x.Select(c => int.Parse(c.ToString())))
                .ToTwoDimensionalArray(),
            data => CalculateLowestPointsParallel(data).Sum(x => data[x.X, x.Y] + 1));

    public static AdventAssignment PartTwo =>
        AdventAssignment.Build(
            InputFile,
            input => input.Split(Environment.NewLine).Select(x => x.Select(c => int.Parse(c.ToString())))
                .ToTwoDimensionalArray(),
            data => CalculateLowestPointsParallel(data).Select(x => CalculateBasinSize(data, x))
                .OrderByDescending(x => x).Take(3).Aggregate(1, (a, b) => a * b));

    public static ParallelQuery<(int X, int Y)> CalculateLowestPointsParallel(int[,] inputs)
    {
        var width = inputs.GetUpperBound(0) + 1;
        var height = inputs.GetUpperBound(1) + 1;

        return Enumerable.Range(0, width).AsParallel().SelectMany(currentWidth =>
            Enumerable.Range(0, height).AsParallel().Select(currentHeight =>
            {
                var current = inputs[currentWidth, currentHeight];

                var neighbors = new[]
                {
                    inputs.TryGet(currentWidth - 1, currentHeight),
                    inputs.TryGet(currentWidth + 1, currentHeight),
                    inputs.TryGet(currentWidth, currentHeight - 1),
                    inputs.TryGet(currentWidth, currentHeight + 1),
                };

                if (neighbors.WhereNotNull().All(x => x > current))
                {
                    return ((int X, int Y)?) (currentWidth, currentHeight);
                }

                return null;
            }).WhereNotNull());
    }

    public static int CalculateBasinSize(int[,] inputs, (int X, int Y) startingPoint)
    {
        var visitedPoints = new ConcurrentBag<(int X, int Y)>();
        var pointsToVisit = new ConcurrentQueue<(int X, int Y)>();

        pointsToVisit.Enqueue(startingPoint);
        var sum = 0;

        while (pointsToVisit.TryDequeue(out var currentPoint))
        {
            if (visitedPoints.Contains(currentPoint))
            {
                continue;
            }

            sum++;
            visitedPoints.Add(currentPoint);

            var currentValue = inputs[currentPoint.X, currentPoint.Y];

            var neighbors = new[]
            {
                (X: currentPoint.X - 1, currentPoint.Y),
                (X: currentPoint.X + 1, currentPoint.Y),
                (currentPoint.X, Y: currentPoint.Y - 1),
                (currentPoint.X, Y: currentPoint.Y + 1)
            };

            foreach (var neighbor in neighbors)
            {
                var val = inputs.TryGet(neighbor.X, neighbor.Y);
                if (val > currentValue && val != 9)
                {
                    pointsToVisit.Enqueue(neighbor);
                }
            }
        }

        return sum;
    }
}