using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Puzzles.Day1;
using AdventOfCode.Puzzles.Day2;
using AdventOfCode.Puzzles.Day3;
using AdventOfCode.Puzzles.Day4;
using AdventOfCode.Puzzles.Day5;
using AdventOfCode.Puzzles.Day6;
using AdventOfCode.Puzzles.Day7;
using AdventOfCode.Puzzles.Day8;

namespace AdventOfCode.Runner
{

    public class AdventRunner
    {
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
	    };

        public void Run()
        {
	        foreach (var line in _days.SelectMany(x => x.Run()))
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