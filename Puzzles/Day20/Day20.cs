using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles.Day20;

public class Day20 : AdventDay
{
    private const string InputFile = "Day20/day20.txt";

    private const string TestInput =
        @"..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#

#..#.
#....
##..#
..#..
..###";

    public Day20()
        : base(20, AdventDayImplementation.Build(AdventDataSource.FromRaw(TestInput), Parse, PartOne))
    {
    }

    private static ScannerData Parse(string input) => ScannerData.Parse(input);

    private static string PartOne(ScannerData data)
    {
        data = GrowBoard(data, 5, out _, out _);

        Console.WriteLine(data.Input.Render(x => x ? '#' : '.'));

        data = Step(data);

        Console.WriteLine(data.Input.Render(x => x ? '#' : '.'));

        data = Step(data);

        Console.WriteLine(data.Input.Render(x => x ? '#' : '.'));

        return data.Input.Flatten().AsParallel().Count(x => x).ToString();
    }

    private static string PartTwo(string data) => data;

    private static ScannerData Step(ScannerData scannerData)
    {
        var increment = 1;
        var biggerData = GrowBoard(scannerData, increment, out var width, out var height);

        var lol = GrowBoard(scannerData, 0, out _, out _);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                biggerData.Input[x, y] = CalculateState(lol, new Point2D(x, y));
            }
        }

        return biggerData with { BorderPixel = biggerData.Input[0, 0] };
    }

    public static ScannerData GrowBoard(ScannerData scannerData, int increment, out int width, out int height)
    {
        width = scannerData.Input.GetLength(0) + 2 * increment;
        height = scannerData.Input.GetLength(1) + 2 * increment;

        var newBoard = new bool[width, height];
        newBoard.Fill(scannerData.BorderPixel);

        newBoard = newBoard.Paste(scannerData.Input, increment, increment);

        var resultBoard = new bool[width, height];
        resultBoard.Paste(newBoard, 0, 0);

        return new ScannerData(scannerData.EnhanceAlgorithm, resultBoard, scannerData.BorderPixel);
    }

    public static bool CalculateState(ScannerData data, Point2D point)
    {
        var reslt = CalculateEnhanceSpot(data.Input, data.BorderPixel, point);
        return data.EnhanceAlgorithm.Get(reslt);
    }

    public static int CalculateEnhanceSpot(bool[,] board, bool borderPixel, Point2D point)
    {
        var pixels = point.GetSurrounding().ToArray();

        var values = pixels.Select(x => board.GetPointOr(x, borderPixel)).ToArray();

        var s = string.Join(string.Empty, values.Select(x => x ? '1' : '0'));

        var result = 0;

        foreach (var p in values)
        {
            result <<= 1;
            result |= p ? 1 : 0;
        }

        return result;
    }
}