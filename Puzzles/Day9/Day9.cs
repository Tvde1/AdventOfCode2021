using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using AdventOfCode.Common;
using Microsoft.VisualBasic.CompilerServices;

namespace AdventOfCode.Puzzles.Day9
{
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
                input => input.Split(Environment.NewLine).Select(x => x.ToCharArray().Select(c => int.Parse(c.ToString()))).ToTwoDimensionalArray(),
                data => CalculateLowestPoints(data).Sum(x => data[x.X, x.Y] + 1));

        public static AdventAssignment PartTwo =>
            AdventAssignment.Build(
                InputFile,
                input => input.Split(Environment.NewLine)
                    .Select(x => x.ToCharArray().Select(c => int.Parse(c.ToString()))).ToTwoDimensionalArray(),
                data => CalculateLowestPoints(data).Select(x => CalculateBasinSize(data, x).Item1)
                    .OrderByDescending(x => x).Take(3).Aggregate(1, (a, b) => a * b));

        public static IEnumerable<(int X, int Y)> CalculateLowestPoints(int[,] inputs)
        {
            var width = inputs.GetUpperBound(0);
            var height = inputs.GetUpperBound(1);

            for (var currentHeight = 0; currentHeight <= height; currentHeight++)
                for (var currentWidth = 0; currentWidth <= width; currentWidth++)
                {
                    var current = inputs[currentWidth, currentHeight];

                    var neighbors = new[]
                    {
                        inputs.TryGet(currentWidth - 1, currentHeight) ?? int.MaxValue,
                        inputs.TryGet(currentWidth + 1, currentHeight) ?? int.MaxValue,
                        inputs.TryGet(currentWidth, currentHeight - 1) ?? int.MaxValue,
                        inputs.TryGet(currentWidth, currentHeight + 1) ?? int.MaxValue,
                    };

                    if (neighbors.All(x => x > current))
                    {
                        yield return (currentWidth, currentHeight);
                    }
                }
        }

        public static (int, List<(int X, int Y)>) CalculateBasinSize(int[,] inputs, (int X, int Y) currentPoint, List<(int X, int Y)>? visitedPoints = null)
        {
            visitedPoints ??= new List<(int X, int Y)>();

            if (visitedPoints.Contains(currentPoint))
            {
                return (0, visitedPoints);
            }

            visitedPoints.Add(currentPoint);

            var current = inputs[currentPoint.X, currentPoint.Y];

            var neighbors = new (int X, int Y)[]
            {
                (X: currentPoint.X - 1, Y: currentPoint.Y),
                (X: currentPoint.X + 1, Y: currentPoint.Y),
                (X: currentPoint.X, Y: currentPoint.Y - 1),
                (X: currentPoint.X, Y: currentPoint.Y + 1),
            };

            var sum = 1;
            foreach (var neighbor in neighbors)
            {
                var val = inputs.TryGet(neighbor.X, neighbor.Y);
                if (val > current && val != 9)
                {
                    var res = CalculateBasinSize(inputs, neighbor, visitedPoints);
                    sum += res.Item1;
                    visitedPoints = visitedPoints.Union(res.Item2).ToList();
                }
            }

            return (sum, visitedPoints);
        }
    }
}