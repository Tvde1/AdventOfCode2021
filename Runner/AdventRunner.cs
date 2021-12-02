using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Puzzles.Day1;
using AdventOfCode.Puzzles.Day2;

namespace AdventOfCode.Runner
{

    public class AdventRunner
    {
	    private readonly List<AdventDayBase> _days = new()
	    {
            new Day1(),
            new Day2(),
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