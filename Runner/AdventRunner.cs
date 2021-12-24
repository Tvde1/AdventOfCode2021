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
using AdventOfCode.Puzzles.Day17;
using AdventOfCode.Puzzles.Day18;
using AdventOfCode.Puzzles.Day19;
using AdventOfCode.Puzzles.Day20;
using AdventOfCode.Puzzles.Day21;
using AdventOfCode.Puzzles.Day22;
//using AdventOfCode.Puzzles.Day23;
using AdventOfCode.Puzzles.Day24;

namespace AdventOfCode.Runner
{
    public class AdventRunner
    {
        private readonly List<int>? _daysToSkip = null;//new() { 9, 11, 15, };
        private readonly int? _onlyDay = 24;

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
            new Day17(),
            new Day18(),
            new Day19(),
            new Day20(),
            new Day21(),
            new Day22(),
            //new Day23(),
            new Day24(),
        };

        public void Run()
        {
            List<AdventDay> days;
            if (_onlyDay.HasValue)
            {
                days = _days.Where(x => x.DayNumber == _onlyDay.Value)
                    .ToList();
            }
            else if (_daysToSkip is not null)
            {
                days = _days
                    .Where(x => !_daysToSkip.Contains(x.DayNumber))
                    .ToList();
            } 
            else
            {
                days = _days;
            }

            foreach (var day in days)
            {
                var result = day.Implementation.Run();
                Console.WriteLine($"Day: {day.DayNumber:D2} | Parsing took {result.ParseDurationMs}ms");

                var i  = 1;
                foreach(var partResult in result.PartResults)
                {
                    Console.WriteLine($"Part {i} took {partResult.ExecutionDurationMs}ms.");
                    Console.WriteLine(partResult.Output);
                    i++;
                }
            }

            Console.WriteLine("End of advent...");
        }

        public static AdventRunner Build()
        {
            return new AdventRunner();
        }
    }
}