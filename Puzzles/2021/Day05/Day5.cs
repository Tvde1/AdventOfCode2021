using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles._2021.Day05;

public class Day5 : AdventDay
{
    private const string InputFile = "Day05/day5.txt";

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
        : base(5, AdventDayImplementation.Build(AdventDataSource.FromFile(InputFile), Parse, PartOne, PartTwo))
    { }

    public readonly struct VentLine
    {
        private readonly Point2D _start;
        private readonly Point2D _end;

        private VentLine(Point2D start, Point2D end)
        {
            _start = start;
            _end = end;
        }

        public static VentLine Parse(string input)
        {
            var s = input.Split(" -> ");
            var startVector = Point2D.Parse(s[0]);
            var endVector = Point2D.Parse(s[1]);
            return new VentLine(startVector, endVector);
        }

        public Point2D[] CalculateAllCoveringPoints(bool countDiagonals)
        {
            var isVertical = _start.X == _end.X;
            var isHorizontal = _start.Y == _end.Y;

            if (isVertical)
            {
                var x = _start.X;
                var smallest = Math.Min(_start.Y, _end.Y);
                var largest = Math.Max(_start.Y, _end.Y);

                return Enumerable.Range(smallest, largest - smallest + 1)
                    .Select(y => new Point2D(x, y))
                    .ToArray();
            }

            if (isHorizontal)
            {
                var y = _start.Y;
                var smallest = Math.Min(_start.X, _end.X);
                var largest = Math.Max(_start.X, _end.X);

                return Enumerable.Range(smallest, largest - smallest + 1)
                    .Select(x => new Point2D(x, y))
                    .ToArray();
            }

            if (countDiagonals)
            {
                return GenerateDiagonals(_start, _end).ToArray();
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


    public static IEnumerable<VentLine> Parse(string input) =>
        input.Split(Environment.NewLine).Select(VentLine.Parse);


    // 2743844
    public static string PartOne(IEnumerable<VentLine> data) => FillBoardAndCount(data, false);

    public static string PartTwo(IEnumerable<VentLine> data) => FillBoardAndCount(data, true);

    private static string FillBoardAndCount(IEnumerable<VentLine> data, bool countDiagonals)
    {
        var board = new Dictionary<Point2D, int>();

        foreach (var point in data.SelectMany(x => x.CalculateAllCoveringPoints(countDiagonals)))
            if (board.ContainsKey(point))
                board[point]++;
            else
                board.Add(point, 1);

        return board.Values.Count(x => x > 1).ToString();
    } 
}