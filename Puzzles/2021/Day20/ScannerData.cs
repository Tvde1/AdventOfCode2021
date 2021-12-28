using System;
using System.Collections;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles._2021.Day20;

public readonly record struct ScannerData(BitArray EnhanceAlgorithm, bool[,] Input, bool BorderPixel)
{
    public static ScannerData Parse(string input)
    {
        var spl = input.Split(Environment.NewLine);

        var algo = spl[0].Select(x => x == '#').ToArray();

        var grid = spl.Skip(2).Select(x => x.Select(y => y == '#')).ToTwoDimensionalArray();

        return new ScannerData(new BitArray(algo), grid, false);
    }
}