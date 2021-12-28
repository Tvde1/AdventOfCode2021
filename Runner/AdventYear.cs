using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Common;
using AdventOfCode.Puzzles._2021.Day01;

namespace AdventOfCode.Runner
{
    internal class AdventYear
    {
        public AdventYear(int year)
        {
            Year = year;

            Days = GetDays(year);
        }

        public int Year { get; }

        public IReadOnlyList<AdventDay> Days { get; }


        public static List<AdventYear> GetYears()
        {
            return Assembly.GetAssembly(typeof(Day1))!.GetTypes()
                .Where(x => x.IsAssignableTo(typeof(AdventDay)))
                .Select(x => x.Namespace!)
                .Select(x => x.Split('.'))
                .Select(x => x[2][1..])
                .Distinct()
                .Select(x => new AdventYear(int.Parse(x)))
                .OrderBy(x => x.Year)
                .ToList();
        }

        private static IReadOnlyList<AdventDay> GetDays(int year)
        {
            var namespaceToFind = $"Puzzles._{year:D4}";

            return Assembly.GetAssembly(typeof(Day1))!.GetTypes()
                .Where(x => x.IsAssignableTo(typeof(AdventDay)))
                .Where(x => x.Namespace!.Contains(namespaceToFind))
                .Select(type => (AdventDay) Activator.CreateInstance(type)!)
                .OrderBy(x => x.DayNumber)
                .ToList();
        }
    }
}
