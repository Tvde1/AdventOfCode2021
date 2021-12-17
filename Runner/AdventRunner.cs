using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Puzzles.Day01;
using AdventOfCode.Puzzles.Day02;
using AdventOfCode.Puzzles.Day03;
using AdventOfCode.Puzzles.Day04;
using AdventOfCode.Puzzles.Day05;
using AdventOfCode.Puzzles.Day06;
using AdventOfCode.Puzzles.Day07;
using AdventOfCode.Puzzles.Day08;
using AdventOfCode.Puzzles.Day09;
using AdventOfCode.Puzzles.Day10;
using AdventOfCode.Puzzles.Day11;
using AdventOfCode.Puzzles.Day12;
using AdventOfCode.Puzzles.Day13;
using AdventOfCode.Puzzles.Day14;
using AdventOfCode.Puzzles.Day15;
using AdventOfCode.Puzzles.Day16;

namespace AdventOfCode.Runner
{
    public class AdventRunner
    {
        private readonly List<int> _daysToSkip = new() { 9, 11, 15, };
        private readonly int? _onlyDay = 15;

        private readonly List<AdventDay> _days = new()
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
            new Day13(),
            new Day14(),
            new Day15(),
            new Day16(),
        };

        public void Run()
        {
            List<AdventDay> days;
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