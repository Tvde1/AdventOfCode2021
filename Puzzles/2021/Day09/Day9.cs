using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles._2021.Day09;

public class Day9 : AdventDay
{
    private const string TestInput =
        @"2199943210
3987894921
9856789892
8767896789
9899965678";

    public Day9()
        : base(AdventDayImplementation.Build(AdventDataSource.ForThisDay(), Parse, PartOne, PartTwo))
    { }

    private static int[,] Parse(string input) => 
        input.Split(Environment.NewLine).Select(x => x.Select(c => int.Parse(c.ToString()))).ToTwoDimensionalArray();

    public static string PartOne(int[,] data) => CalculateLowestPointsParallel(data).Sum(x => data[x.X, x.Y] + 1).ToString();

    public static string PartTwo(int[,] data) => CalculateLowestPointsParallel(data)
        .Select(x => CalculateBasinSize(data, x))
        .OrderByDescending(x => x)
        .Take(3)
        .Aggregate(1, (a, b) => a * b)
        .ToString();

    private static IEnumerable<Point2D> CalculateLowestPointsParallel(int[,] inputs)
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
                            return (Point2D?) new Point2D(currentWidth, currentHeight);
                        }

                        return null;
                    }).WhereNotNull());
    }

    private static int CalculateBasinSize(int[,] inputs, Point2D startingPoint)
    {
        var visitedPoints = new HashSet<Point2D>();
        var pointsToVisit = new Queue<Point2D>();

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
                new Point2D(currentPoint.X - 1, currentPoint.Y),
                new Point2D(currentPoint.X + 1, currentPoint.Y),
                new Point2D(currentPoint.X, currentPoint.Y - 1),
                new Point2D(currentPoint.X, currentPoint.Y + 1)
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