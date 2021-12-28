using System;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles._2021.Day20;

public class Day20 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput =
        AdventDataSource.FromRaw(@"..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#

#..#.
#....
##..#
..#..
..###");

    public Day20()
        : base(AdventDayImplementation.Build(RealInput, Parse, PartOne, PartTwo))
    {
    }

    private static ScannerData Parse(string input) => ScannerData.Parse(input);

    private static string PartOne(ScannerData data)
    {
        data = GrowBoard(data, 5, out _, out _);

        data = Step(data);
        data = Step(data);

        return data.Input.Flatten().AsParallel().Count(x => x).ToString();
    }

    private static string PartTwo(ScannerData data)
    {
        data = GrowBoard(data, 5, out _, out _);

        for (int i = 0; i < 50; i++)
        {
            data = Step(data);
        }

        Console.Write(data.Input.Render(x => x ? '#' : '.'));

        return data.Input.Flatten().AsParallel().Count(x => x).ToString();
    }

    private static ScannerData Step(ScannerData scannerData)
    {
        var increment = 1;
        var biggerData = GrowBoard(scannerData, increment, out var width, out var height);

        var newBoard = CalcNewState(biggerData);

        return biggerData with { Input = newBoard, BorderPixel = newBoard[0, 0] };
    }

    public static bool[,] CalcNewState(ScannerData scannerData)
    {
        var copy = GrowBoard(scannerData, 0, out var width, out var height);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                copy.Input[x, y] = CalculateState(scannerData, new Point2D(x, y));
            }
        }

        return copy.Input;
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