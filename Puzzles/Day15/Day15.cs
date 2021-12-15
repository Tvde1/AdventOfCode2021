using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            data =>
            {
                var startPoint = new Point2D(0, 0);
                var endPoint = new Point2D(data.GetUpperBound(0), data.GetUpperBound(1));

                var (Path, Cost) = GetCost(data, startPoint, endPoint);

                PrintBoard(data, Path);

                return Cost;
            });

    public static AdventAssignment PartTwo =>
        AdventAssignment.Build(
            InputFile,
            input => GetBigGrid(GetGrid(input), 5),
            data =>
            {
                var startPoint = new Point2D(0, 0);
                var endPoint = new Point2D(data.GetUpperBound(0), data.GetUpperBound(1));

                var (Path, Cost) = GetCost(data, startPoint, endPoint);

                PrintBoard(data, Path);

                return Cost;
            });

    private static int[,] GetGrid(string input) => input.Split(Environment.NewLine).Select(x => x.Select(cha => int.Parse(cha.ToString()))).ToTwoDimensionalArray();

    private static int[,] GetBigGrid(int[,] grid, int timesBigger)
    {
        var width = grid.GetLength(0);
        var height = grid.GetLength(1);

        int newWidth = grid.GetLength(0) * timesBigger;
        int newHeight = grid.GetLength(1) * timesBigger;

        var newGrid = new int[newWidth, newHeight];

        foreach (var currentX in Enumerable.Range(0, newWidth))
        {
            foreach (var currentY in Enumerable.Range(0, newHeight))
            {
                var (quotientX, remainderX) = Math.DivRem(currentX, width);
                var (quotientY, remainderY) = Math.DivRem(currentY, height);

                var valueAt = grid[remainderX, remainderY];

                var newValue = valueAt + quotientX + quotientY;

                newValue = (newValue - 1) % 9 + 1;

                newGrid[currentX, currentY] = newValue;
            }
        }

        return newGrid;
    }

    private static (Point2D[] Path, int Cost) GetCost(int[,] grid, Point2D start, Point2D end)
    {
        var rightBound = grid.GetUpperBound(0);
        var bottomBound = grid.GetUpperBound(1);

        var visitedPoints = new HashSet<Point2D>();
        var openPoints = new PriorityQueue<Point2D, int>();
        openPoints.Enqueue(start, 0);

        var breadCrumbs = new Dictionary<Point2D, (Point2D Point, int Cost)>();

        while (openPoints.TryDequeue(out var lowestCostPoint, out var lowestCostValue))
        {
            visitedPoints.Add(lowestCostPoint);

            var neighbors = GetNeighbors(lowestCostPoint, rightBound, bottomBound);

            foreach (var neighbor in neighbors)
            {
                if (visitedPoints.Contains(neighbor))
                {
                    continue;
                }

                var newCost = lowestCostValue + grid.GetPoint(neighbor);

                if (!breadCrumbs.TryGetValue(neighbor, out var crumb) || crumb.Cost > lowestCostValue)
                {
                    breadCrumbs[neighbor] = (lowestCostPoint, lowestCostValue);
                }


                if (neighbor == end)
                {
                    return (GetPath(breadCrumbs, end), newCost);
                }

                openPoints.Enqueue(neighbor, newCost);
            }
        }

        throw new Oopsie("No possible paths");
    }

    private static IEnumerable<Point2D> GetNeighbors(Point2D point, int rightBound, int bottomBound)
    {
        // Left
        if (point.X > 0)
        {
            yield return point with { X = point.X - 1 };
        }

        // Up
        if (point.Y > 0)
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

    static void PrintBoard(int[,] board, Point2D[] path)
    {
        var width = board.GetLength(0);
        var height = board.GetLength(1);
        foreach (var y in Enumerable.Range(0, height))
        {
            foreach (var x in Enumerable.Range(0, width))
            {
                if (path.Contains(new Point2D(x, y)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.Write(board[x, y]);

                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine();
        }
    }

    private static Point2D[] GetPath(Dictionary<Point2D, (Point2D Point, int Cost)> breadCrumbs, Point2D currentPoint)
    {
        var path = new List<Point2D> { currentPoint };

        while (breadCrumbs.TryGetValue(currentPoint, out var newPoint))
        {
            path.Add(newPoint.Point);
            currentPoint = newPoint.Point;
        }

        return path.AsEnumerable().Reverse().ToArray();
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