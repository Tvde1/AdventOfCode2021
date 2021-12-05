using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day3
{
	public class Day3 : AdventDayBase
	{
		private const string InputFile = "Day3/day3.txt";

		private const string testInput = @"00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010";

		public Day3()
			: base(3)
		{
			AddPart(PartOne);
			AddPart(BuildPartTwo);
		}

		// 2743844
		public static AdventAssignment PartOne =>
			AdventAssignment.Build(
				InputFile,
				input => input.Split(Environment.NewLine).ToArray(),
				data =>
				{
					var aggregate = new int[data[0].Length];

					foreach (var line in data)
						for (var index = 0; index < line.Length; index++)
							aggregate[index] += line[index] == '1' ? 1 : -1;

					var result = aggregate.Aggregate(0, (agg, cur) =>
					{
						agg <<= 1;
						if (cur > 0)
						{
							agg |= 1;
						}

						return agg;
					});

					var inverseResult = ~result & 0b1111_1111_1111;

					return (result * inverseResult).ToString().Enumerate();
				});

		// 6677951
		public static AdventAssignment BuildPartTwo => AdventAssignment.Build(
			InputFile,
			input => input.Split(Environment.NewLine).ToArray(),
			data =>
			{
				var mostCommon = Convert.ToInt32(FindMostCommon(data, false), 2);
				var leastCommon = Convert.ToInt32(FindMostCommon(data, true), 2);

				return (mostCommon * leastCommon).ToString().Enumerate();
			});

		public static string FindMostCommon(string[] input, bool inverse, int indexToMatch = 0)
		{
			var grouped = input.ToLookup(x => x[indexToMatch])
				.OrderBy(x => x.Count())
				.ThenBy(x => x.Key);

			var set = (!inverse ? grouped.Last() : grouped.First()).ToArray();

			if (set.Length == 1) return set[0];

			return FindMostCommon(set, inverse, indexToMatch + 1);
		}
	}
}