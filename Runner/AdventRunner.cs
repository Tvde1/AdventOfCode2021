using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Puzzles.Day1;
using AdventOfCode.Puzzles.Day10;
using AdventOfCode.Puzzles.Day11;
using AdventOfCode.Puzzles.Day12;
using AdventOfCode.Puzzles.Day2;
using AdventOfCode.Puzzles.Day3;
using AdventOfCode.Puzzles.Day4;
using AdventOfCode.Puzzles.Day5;
using AdventOfCode.Puzzles.Day6;
using AdventOfCode.Puzzles.Day7;
using AdventOfCode.Puzzles.Day8;
using AdventOfCode.Puzzles.Day9;

namespace AdventOfCode.Runner
{
    public class AdventRunner
    {
        private readonly List<int> _daysToSkip = new() { 9, 11 };
        private readonly int? _onlyDay = 12;

        private readonly List<AdventDayBase> _days = new()
        {
            new Day1(),
            new Day2(),
            new Day3(),
            new Day4(),
            new Day5(),
            new Day6(),
            new Day7(),
            new Day8(),
            new Day9(),
            new Day10(),
            new Day11(),
            new Day12(),
        };

        public void Run()
        {
            List<AdventDayBase> days;
            if (_onlyDay.HasValue)
            {
                days = _days.Where(x => x.Number == _onlyDay.Value)
                    .ToList();
            }
            else
            {
                days = _days
                    .Where(x => !_daysToSkip.Contains(x.Number))
                    .ToList();
            }

            foreach (var line in days.SelectMany(x => x.Run()))
            {
                Console.WriteLine(line);
            }

            Console.WriteLine("End of advent...");
        }

        public static AdventRunner Build()
        {
            return new AdventRunner();
        }
    }
}