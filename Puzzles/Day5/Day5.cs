using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;

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
                var board = new Dictionary<Vector2, int>();

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

    private static Dictionary<Vector2, int> FillBoard(IEnumerable<VentLine> data)
    {
        var board = new Dictionary<Vector2, int>();

        foreach (var point in data.SelectMany(x => x.AllCoveringPoints))
            if (board.ContainsKey(point))
                board[point]++;
            else
                board.Add(point, 1);

        return board;
    }


    private readonly record struct Vector2(int X, int Y);

    private readonly struct VentLine
    {
        private readonly Vector2 _start;
        private readonly Vector2 _end;

        private VentLine(Vector2 start, Vector2 end, bool countDiagonals)
        {
            _start = start;
            _end = end;
            AllCoveringPoints = CalculateAllCoveringPoints(start, end, countDiagonals);
        }

        public Vector2[] AllCoveringPoints { get; }

        public static Func<string, VentLine> Parse(bool countDiagonals)
        {
            return input =>
            {
                var s = input.Split(" -> ");
                var start = s[0].Split(",");
                var startVector = new Vector2(int.Parse(start[0]), int.Parse(start[1]));
                var end = s[1].Split(",");
                var endVector = new Vector2(int.Parse(end[0]), int.Parse(end[1]));
                return new VentLine(startVector, endVector, countDiagonals);
            };
        }

        private static Vector2[] CalculateAllCoveringPoints(Vector2 start, Vector2 end, bool countDiagonals)
        {
            var isVertical = start.X == end.X;
            var isHorizontal = start.Y == end.Y;

            if (isVertical)
            {
                var x = start.X;
                var smallest = Math.Min(start.Y, end.Y);
                var largest = Math.Max(start.Y, end.Y);

                return Enumerable.Range(smallest, largest - smallest + 1)
                    .Select(y => new Vector2(x, y))
                    .ToArray();
            }

            if (isHorizontal)
            {
                var y = start.Y;
                var smallest = Math.Min(start.X, end.X);
                var largest = Math.Max(start.X, end.X);

                return Enumerable.Range(smallest, largest - smallest + 1)
                    .Select(x => new Vector2(x, y))
                    .ToArray();
            }

            if (countDiagonals)
            {
                return GenerateDiagnoals(start, end).ToArray();
            }

            return Array.Empty<Vector2>();
        }

        private static IEnumerable<Vector2> GenerateDiagnoals(Vector2 start, Vector2 end)
        {
            var goingRight = start.X < end.X;
            var goingDown = start.Y < end.Y;

            var y = start.Y;
            var x = start.X;

            while (x != end.X)
            {
                yield return new Vector2(x, y);

                if (goingDown)
                    y++;
                else
                    y--;

                if (goingRight)
                    x++;
                else
                    x--;
            }

            yield return new Vector2(x, y);
        }
    }
}