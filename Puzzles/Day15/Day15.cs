using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles.Day15;

public class Day15 : AdventDayBase
{
    private const string InputFile = "Day15/day15.txt";

    private const string TestInput = @"1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581";

    public Day15()
        : base(15)
    {
        AddPart(PartOne);
        AddPart(PartTwo);
    }

    public static AdventAssignment PartOne =>
        AdventAssignment.Build(
            InputFile,
            GetGrid,
            data => GetCost(data, new Point2D(0, 0), new Point2D(data.GetUpperBound(0), data.GetUpperBound(1))));

    public static AdventAssignment PartTwo =>
        AdventAssignment.Build(
            InputFile,
            input => GetBigGrid(GetGrid(TestInput), 5),
            data => GetCost(data, new Point2D(0, 0), new Point2D(data.GetUpperBound(0), data.GetUpperBound(1))));

    private static int[,] GetGrid(string input) => input.Split(Environment.NewLine).Select(x => x.Select(cha => int.Parse(cha.ToString()))).ToTwoDimensionalArray();

    private static int[,] GetBigGrid(int[,] grid, int timesBigger)
    {
        var rightBound = grid.GetUpperBound(0);
        var bottomBound = grid.GetUpperBound(1);

        var newGrid = new int[(rightBound + 1) * timesBigger, (bottomBound + 1) * timesBigger];

        foreach (var currentX in Enumerable.Range(0, newGrid.GetLength(0)))
        foreach (var currentY in Enumerable.Range(0, newGrid.GetLength(1)))
        {
            var (quotientX, remainderX) = Math.DivRem(currentX, rightBound + 1);
            var (quotientY, remainderY) = Math.DivRem(currentY, bottomBound + 1);

            var valueAt = grid[remainderX, remainderY];

            var newValue = valueAt + quotientX + quotientY;

            while (newValue > 9)
            {
                newValue -= 9;
            }

            newGrid[currentX, currentY] = newValue;
        }

        return newGrid;
    }

    private static int GetCost(int[,] grid, Point2D start, Point2D end)
    {
        var rightBound = grid.GetUpperBound(0);
        var bottomBound = grid.GetUpperBound(1);

        var visitedPoints = new HashSet<Point2D>();
        var openPoints = new Dictionary<Point2D, int> { { start, 0 } };

        while (openPoints.Count > 0)
        {
            var (lowestCostPoint, lowestCostValue) = openPoints.OrderBy(x => x.Key).First();

            openPoints.Remove(lowestCostPoint);
            visitedPoints.Add(lowestCostPoint);

            var neighbors = GetNeighbors(lowestCostPoint, rightBound, bottomBound);

            foreach (var neighbor in neighbors)
            {
                if (visitedPoints.Contains(neighbor))
                {
                    continue;
                }

                var newCost = lowestCostValue + grid.GetPoint(neighbor);

                if (neighbor == end)
                {
                    return newCost;
                }

                var doInsertPoint = true;

                if (openPoints.TryGetValue(neighbor, out var existingNeighbor))
                {
                    doInsertPoint = existingNeighbor > newCost;

                    if (doInsertPoint)
                    {
                        openPoints.Remove(neighbor);
                    }
                }

                if (doInsertPoint)
                {
                    openPoints.Add(neighbor, newCost);
                }
            }
        }

        throw new Oopsie("No possible paths");
    }

    private static IEnumerable<Point2D> GetNeighbors(Point2D point, int rightBound, int bottomBound)
    {
        // Left
        if (point.X > 1)
        {
            yield return point with { X = point.X - 1 };
        }

        // Up
        if (point.Y > 1)
        {
            yield return point with { Y = point.Y - 1 };
        }

        // Right
        if (point.X < rightBound)
        {
            yield return point with { X = point.X + 1 };
        }

        // Down
        if (point.Y < bottomBound)
        {
            yield return point with { Y = point.Y + 1 };
        }
    }

    static int ComputeHScore(int x, int y, int targetX, int targetY)
    {
        return Math.Abs(targetX - x) + Math.Abs(targetY - y);
    }

    private class Oopsie : Exception
    {
        public Oopsie()
            : base("This should not be possible")
        {
        }

        public Oopsie(string message)
            : base(message)
        {
        }
    }
}