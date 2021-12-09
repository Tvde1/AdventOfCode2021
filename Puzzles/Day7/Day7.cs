using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day7
{
	public class Day7 : AdventDayBase
	{
		private const string InputFile = "Day7/day7.txt";

		private const string TestInput = "16,1,2,0,4,2,7,1,2,14";

		public Day7()
			: base(7)
		{
			AddPart(PartOne);
			AddPart(PartTwo);
		}

		// 355764
		public static AdventAssignment PartOne =>
			AdventAssignment.Build(
				InputFile,
				input => input.Split(",").Select(int.Parse),
				data => FindMedianCrabCost(data));

		// 99634572
		public static AdventAssignment PartTwo =>
			AdventAssignment.Build(
				InputFile,
				input => input.Split(",").Select(int.Parse).ToList(),
				data => FindBruteForceCrabCost(data));


		public static int FindMedianCrabCost(IEnumerable<int> horizontalPositions)
		{
			var sorted = horizontalPositions.OrderBy(x => x).ToList();
			var median = sorted[sorted.Count / 2];

			return CalculateCost(sorted, median);
		}

		public static int FindBruteForceCrabCost(IEnumerable<int> horizontalPositions)
		{
			var sorted = horizontalPositions.OrderBy(x => x).ToList();

			var range = sorted.Min()..sorted.Max();

			List<int> costs = new();
			foreach (var x in range)
			{
				costs.Add(CalculateCostTwo(sorted, x));
			}

			return costs.Min();
		}

		public static int CalculateCost(IEnumerable<int> horizontalPositions, int alignPosition)
		{
			return horizontalPositions.Sum(x => Math.Abs(alignPosition - x));
		}

		public static int CalculateCostTwo(IEnumerable<int> horizontalPositions, int alignPosition)
		{
			return horizontalPositions.Sum(x =>
			{
				var n = Math.Abs(alignPosition - x);
				return (int) (Math.Pow(n, 2) + n) / 2;
			});
		}
	}
}