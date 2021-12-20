using System;
using System.Collections;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day20;

public readonly record struct ScannerData(BitArray EnhanceAlgorithm, bool[,] Input, bool BorderPixel)
{
    public static ScannerData Parse(string input)
    {
        var spl = input.Split(Environment.NewLine);

        var algo = spl[0].Select(x => x == '#').ToArray();

        var grid = spl.Skip(2).Select(x => x.Select(y => y == '#')).ToTwoDimensionalArray().Flip();

        return new ScannerData(new BitArray(algo), grid, false);
    }
}