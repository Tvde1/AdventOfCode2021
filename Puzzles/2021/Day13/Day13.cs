using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles._2021.Day13;

public class Day13 : AdventDay
{
    private const string TestInput = @"6,10
0,14
9,10
0,3
10,4
4,11
6,0
6,12
4,1
0,13
10,12
3,4
3,0
8,4
1,10
2,14
8,10
9,0

fold along y=7
fold along x=5";

    public Day13()
        : base(AdventDayImplementation.Build(AdventDataSource.ForThisDay(), Sheet.Parse, PartOne, PartTwo))
    { }

    public static string PartOne(Sheet data) => FoldSheet(data).Points.Count.ToString();

    public static string PartTwo(Sheet data)
    {
        while (data.Folds.Any())
        {
            data = FoldSheet(data);
        }

        return RenderSheet(data);
    }

    public readonly record struct Sheet
    {
        public ISet<Point2D> Points { get; init; }

        public IReadOnlyList<FoldOperation> Folds { get; init; }

        public static Sheet Parse(string input)
        {
            var lines = input.Split(Environment.NewLine);

            var points = new HashSet<Point2D>();
            var folds = new List<FoldOperation>();

            var parseFolds = false;
            foreach (var line in lines)
            {
                if (parseFolds)
                {
                    folds.Add(FoldOperation.Parse(line));
                }
                else
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        parseFolds = true;
                        continue;
                    }
                    points.Add(Point2D.Parse(line));
                }
            }

            return new Sheet
            {
                Points = points,
                Folds = folds,
            };
        }
    }

    public enum Axis { X, Y }

    public readonly record struct FoldOperation(Axis Axis, int Value)
    {
        public static FoldOperation Parse(string input)
        {
            var sp = input.Split('=');
            var axis = sp[0][^1] switch
            {
                'x' => Axis.X,
                'y' => Axis.Y,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new FoldOperation(axis, int.Parse(sp[1]));
        }
    }

    public static Sheet FoldSheet(Sheet input)
    {
        var (axis, foldValue) = input.Folds[0];
        var remainingFolds = input.Folds.Skip(1).ToList();

        Func<Point2D, bool> pointSelector;
        Func<Point2D, Point2D> pointMapper;

        switch (axis)
        {
            case Axis.X:
                pointSelector = p => p.X > foldValue;
                pointMapper = p => p with { X = foldValue - (p.X - foldValue) };
                break;
            case Axis.Y:
                pointSelector = p => p.Y > foldValue;
                pointMapper = p => p with { Y = foldValue - (p.Y - foldValue) };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var pointsToMap = input.Points.Where(pointSelector).ToList();
        var newPoints = pointsToMap.Select(pointMapper);

        var remainingPoints = input.Points.Except(pointsToMap).Concat(newPoints).ToHashSet();

        return new Sheet
        {
            Folds = remainingFolds,
            Points = remainingPoints,
        };
    }

    public static string RenderSheet(Sheet input)
    {
        var minX = input.Points.Min(x => x.X);
        var minY = input.Points.Min(x => x.Y);
        var maxX = input.Points.Max(x => x.X);
        var maxY = input.Points.Max(x => x.Y);

        var rendered = Enumerable.Range(minY, maxY - minY + 1).Select(currentY =>
            new string(Enumerable.Range(minX, maxX - minX + 1)
                .Select(currentX => input.Points.Contains(new Point2D(currentX, currentY))
                    ? CharConstants.Filled
                    : CharConstants.Space).ToArray()));

        return string.Join(Environment.NewLine, rendered);
    }
}