using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles.Day5;

public class Day5 : AdventDayBase
{
    private const string InputFile = "Day5/day5.txt";

    private const string TestInput = @"0,9 -> 5,9
8,0 -> 0,8
9,4 -> 3,4
2,2 -> 2,1
7,0 -> 7,4
6,4 -> 2,0
0,9 -> 2,9
3,4 -> 1,4
0,0 -> 8,8
5,5 -> 8,2";

    public Day5()
        : base(5)
    {
        AddPart(PartOne);
        AddPart(PartTwo);
    }

    // 2743844
    public static AdventAssignment PartOne =>
        AdventAssignment.Build(
            InputFile,
            input => input.Split(Environment.NewLine)
                .Select(VentLine.Parse(false)),
            data =>
            {
                var board = new Dictionary<Point2D, int>();

                foreach (var point in data.SelectMany(x => x.AllCoveringPoints))
                    if (board.ContainsKey(point))
                        board[point]++;
                    else
                        board.Add(point, 1);

                return board.Values.Count(x => x > 1);
            });

    public static AdventAssignment PartTwo =>
        AdventAssignment.Build(
            InputFile,
            input => input.Split(Environment.NewLine)
                .Select(VentLine.Parse(true)),
            data =>
            {
                var board = FillBoard(data);

                return board.Values.Count(x => x > 1);
            });

    private static Dictionary<Point2D, int> FillBoard(IEnumerable<VentLine> data)
    {
        var board = new Dictionary<Point2D, int>();

        foreach (var point in data.SelectMany(x => x.AllCoveringPoints))
            if (board.ContainsKey(point))
                board[point]++;
            else
                board.Add(point, 1);

        return board;
    }

    private readonly struct VentLine
    {
        private VentLine(Point2D start, Point2D end, bool countDiagonals)
        {
            AllCoveringPoints = CalculateAllCoveringPoints(start, end, countDiagonals);
        }

        public Point2D[] AllCoveringPoints { get; }

        public static Func<string, VentLine> Parse(bool countDiagonals)
        {
            return input =>
            {
                var s = input.Split(" -> ");
                var startVector = Point2D.Parse(s[0]);
                var endVector = Point2D.Parse(s[1]);
                return new VentLine(startVector, endVector, countDiagonals);
            };
        }

        private static Point2D[] CalculateAllCoveringPoints(Point2D start, Point2D end, bool countDiagonals)
        {
            var isVertical = start.X == end.X;
            var isHorizontal = start.Y == end.Y;

            if (isVertical)
            {
                var x = start.X;
                var smallest = Math.Min(start.Y, end.Y);
                var largest = Math.Max(start.Y, end.Y);

                return Enumerable.Range(smallest, largest - smallest + 1)
                    .Select(y => new Point2D(x, y))
                    .ToArray();
            }

            if (isHorizontal)
            {
                var y = start.Y;
                var smallest = Math.Min(start.X, end.X);
                var largest = Math.Max(start.X, end.X);

                return Enumerable.Range(smallest, largest - smallest + 1)
                    .Select(x => new Point2D(x, y))
                    .ToArray();
            }

            if (countDiagonals)
            {
                return GenerateDiagonals(start, end).ToArray();
            }

            return Array.Empty<Point2D>();
        }

        private static IEnumerable<Point2D> GenerateDiagonals(Point2D start, Point2D end)
        {
            var goingRight = start.X < end.X;
            var goingDown = start.Y < end.Y;

            var y = start.Y;
            var x = start.X;

            while (x != end.X)
            {
                yield return new Point2D(x, y);

                if (goingDown)
                    y++;
                else
                    y--;

                if (goingRight)
                    x++;
                else
                    x--;
            }

            yield return new Point2D(x, y);
        }
    }
}