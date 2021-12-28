using System;

namespace AdventOfCode.Runner
{
    public class AdventRunner
    {
        //private readonly int? _onlyDay = 1;

        private readonly int? _onlyYear = 2020;
        private readonly int? _onlyDay = null;

        public void Run()
        {
            var years = AdventYear.GetYears();

            foreach (var year in years)
            {
                if (_onlyYear.HasValue && _onlyYear.Value != year.Year)
                {
                    continue;
                }

                Console.WriteLine($"Year: {year.Year}");

                foreach (var day in year.Days)
                {
                    if (_onlyDay.HasValue && _onlyDay.Value != day.DayNumber)
                    {
                        continue;
                    }

                    var (parseDurationMs, adventDayPartResults) = day.Implementation.Run();
                    Console.WriteLine($"Day: {day.DayNumber:D2} | Parsing took {parseDurationMs}ms");

                    var i = 1;
                    foreach (var (output, executionDurationMs) in adventDayPartResults)
                    {
                        Console.WriteLine($"Part {i} took {executionDurationMs}ms.");
                        Console.WriteLine(output);
                        i++;
                    }
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