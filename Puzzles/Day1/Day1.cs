using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day1
{
    public class Day1 : AdventDayBase
    {
        const string InputFile = "input.txt";

        public IEnumerable<string> One()
        {
            var lines = File.ReadAllLines(InputFile);

            var count = lines
                .Select(int.Parse)
                .Pairs()
                .Count(x => x.Item1 < x.Item2);

            Console.WriteLine(count);
            yield return count;

            // 1527
        }
    }
}

