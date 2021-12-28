using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles._2021.Day15;

public partial class Day15 : AdventDay
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
        : base(15, AdventDayImplementation.Build(AdventDataSource.FromFile(InputFile), Parse, PartOne, PartTwo, PartThree))
    { }

    public static int[,] Parse(string input) => GetGrid(input);

    public static string PartOne(int[,] data)
    {
        var startPoint = new Point2D(0, 0);
        var endPoint = new Point2D(data.GetUpperBound(0), data.GetUpperBound(1));

        var (Path, Cost) = GetCost(data, startPoint, endPoint);

        return Cost.ToString();
    }
    public static string PartTwo(int[,] data)
    {
        data = GetBigGrid(data, 5);

        var startPoint = new Point2D(0, 0);
        var endPoint = new Point2D(data.GetUpperBound(0), data.GetUpperBound(1));

        var (Path, Cost) = GetCost(data, startPoint, endPoint);

        return Cost.ToString();
    }

    public static string PartThree(int[,] data)
    {
        data = GetBigGrid(data, 5);

        var startPoint = new Point2D(0, 0);
        var endPoint = new Point2D(data.GetUpperBound(0), data.GetUpperBound(1));

        var otherCost = LowestCostUsingAStar(data, startPoint, endPoint);

        return otherCost.ToString();
    }

    private static int[,] GetGrid(string input) => input.Split(Environment.NewLine).Select(x => x.Select(cha => int.Parse(cha.ToString()))).ToTwoDimensionalArray();

    private static int[,] GetBigGrid(int[,] grid, int timesBigger)
    {
        var width = grid.GetLength(0);
        var height = grid.GetLength(1);

        var newWidth = grid.GetLength(0) * timesBigger;
        var newHeight = grid.GetLength(1) * timesBigger;

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

    private static int LowestCostUsingAStar(int[,] grid, Point2D start, Point2D end)
    {
        var rightBound = grid.GetUpperBound(0);
        var bottomBound = grid.GetUpperBound(1);

        var neighbors = new Point2D[4];

        Point2D[] GetNeighbors(Point2D point)
        {
            FillArrayWithNeighbors(point, rightBound, bottomBound, ref neighbors, out var amountOfNeighbors);

            return neighbors.Take(amountOfNeighbors).ToArray();
        }

        var lowestCost = AStar.Calculate(start,
            GetNeighbors,
            (mod, _) => mod,
            grid.GetPoint,
            item => Heuristic(item, end));

        return lowestCost;
    }

    private static (Point2D[] Path, int Cost) GetCost(int[,] grid, Point2D start, Point2D end)
    {
        var rightBound = grid.GetUpperBound(0);
        var bottomBound = grid.GetUpperBound(1);

        var visitedPoints = new HashSet<Point2D>();
        var openPoints = new PriorityQueue<(Point2D Point, int Cost), int>();
        openPoints.Enqueue((start, 0), 0);

        var breadCrumbs = new Dictionary<Point2D, (Point2D Point, int Cost)>();

        var neighbors = new Point2D[4];

        while (openPoints.TryDequeue(out var lowestCostPoint, out _))
        {
            visitedPoints.Add(lowestCostPoint.Point);

            FillArrayWithNeighbors(lowestCostPoint.Point, rightBound, bottomBound,
                ref neighbors, out var neighborCount);

            for (int i = 0; i < neighborCount; i++)
            {
                var neighbor = neighbors[i];

                if (visitedPoints.Contains(neighbor))
                {
                    continue;
                }

                var newCost = lowestCostPoint.Cost + grid.GetPoint(neighbor);
                var heuristicCost = newCost + Heuristic(neighbor, end);

                if (!breadCrumbs.TryGetValue(neighbor, out var crumb) || crumb.Cost > lowestCostPoint.Cost)
                {
                    breadCrumbs[neighbor] = lowestCostPoint;
                }

                if (neighbor == end)
                {
                    return (GetPath(breadCrumbs, end), newCost);
                }

                openPoints.Enqueue((neighbor, newCost), heuristicCost);
            }
        }

        throw new Oopsie("No possible paths");
    }

    private static void FillArrayWithNeighbors(Point2D point, int rightBound, int bottomBound,
        ref Point2D[] neighbors, out int neighborCount)
    {
        neighborCount = 0;

        // Left
        if (point.X > 0)
        {
            neighbors[neighborCount++] = point with { X = point.X - 1 };
        }

        // Up
        if (point.Y > 0)
        {
            neighbors[neighborCount++] = point with { Y = point.Y - 1 };
        }

        // Right
        if (point.X < rightBound)
        {
            neighbors[neighborCount++] = point with { X = point.X + 1 };
        }

        // Down
        if (point.Y < bottomBound)
        {
            neighbors[neighborCount++] = point with { Y = point.Y + 1 };
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

    private static int Heuristic(Point2D point, Point2D endPoint)
    {
        return Math.Abs(endPoint.X - point.X) + Math.Abs(endPoint.Y - point.Y);
    }
}