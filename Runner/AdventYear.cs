using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Common;

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
            var types = Assembly.GetExecutingAssembly().GetTypes();

            var years = types.Where(x =>
                    x.IsClass && x.IsInstanceOfType(typeof(AdventDay)) && x.Namespace!.Contains("Puzzles."))
                .Select(x => x.Namespace!.Split('.')[2][1..]).Distinct();

            return years.Select(x => new AdventYear(int.Parse(x))).OrderBy(x => x).ToList();
        }

        private static IReadOnlyList<AdventDay> GetDays(int year)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();

            var namespaceToFind = $"Puzzles.{year:D4}";

            var typesForYear = types.Where(x => x.IsClass && x.IsInstanceOfType(typeof(AdventDay)) && x.Namespace!.Contains(namespaceToFind));

            var days = typesForYear.Select(type => (AdventDay) Activator.CreateInstance(type)!);

            return days.OrderBy(x => x.DayNumber).ToList();
        }
    }
}
