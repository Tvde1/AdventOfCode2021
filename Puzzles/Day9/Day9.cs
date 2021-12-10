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

    private record Vector2(int X, int Y);

    private static IEnumerable<Vector2> CalculateLowestPointsParallel(int[,] inputs)
    {
        var width = inputs.GetUpperBound(0) + 1;
        var height = inputs.GetUpperBound(1) + 1;

        return Enumerable.Range(0, width)
            .SelectMany(currentWidth =>
                Enumerable.Range(0, height)
                    .Select(currentHeight =>
                    {
                        var current = inputs[currentWidth, currentHeight];

                        var l = inputs.TryGet(currentWidth - 1, currentHeight);
                        var r = inputs.TryGet(currentWidth + 1, currentHeight);
                        var d = inputs.TryGet(currentWidth, currentHeight - 1);
                        var u = inputs.TryGet(currentWidth, currentHeight + 1);

                        if ((l is null || l > current) && (r is null || r > current) && (d is null || d > current) &&
                            (u is null || u > current))
                        {
                            return new Vector2(currentWidth, currentHeight);
                        }

                        return null;
                    }).WhereNotNull());
    }

    private static int CalculateBasinSize(int[,] inputs, Vector2 startingPoint)
    {
        var visitedPoints = new HashSet<Vector2>();
        var pointsToVisit = new Queue<Vector2>();

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
                new Vector2(currentPoint.X - 1, currentPoint.Y),
                new Vector2 (currentPoint.X + 1, currentPoint.Y),
                new Vector2(currentPoint.X, currentPoint.Y - 1),
                new Vector2(currentPoint.X, currentPoint.Y + 1)
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