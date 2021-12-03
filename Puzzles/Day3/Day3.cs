using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day2
{
	public class Day3 : AdventDayBase
	{
		private const string InputFile = "Day3/day3.txt";

		private readonly record struct BitCounter(int Count);

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
			AddPart(BuildPartOne());
			AddPart(BuildPartTwo());
		}

		public static AdventAssignment BuildPartOne()
		{
			// 2743844
			return AdventAssignment.Build(
				1,
				InputFile,
				input => input.Split(Environment.NewLine).ToArray(),
				data =>
				{
					var aggregate = new int[data.First().Length];

					foreach (var line in data)
					{
						for (var index = 0; index < line.Length; index++)
						{
							aggregate[index] += line[index] == '1' ? 1 : -1;
						}
					}

					var aggregateString = new string(aggregate.Select(x => x > 0 ? '1' : '0').ToArray());
					var result = EncodeBools(aggregateString);
					var inverseAggregateString = new string(aggregate.Select(x => x < 0 ? '1' : '0').ToArray());
					var inverseResult = EncodeBools(inverseAggregateString);

					return (result * inverseResult).ToString().Enumerate();
				});
		}

		public static AdventAssignment BuildPartTwo()
		{
			// 6677951
			return AdventAssignment.Build(
				2,
				InputFile,
				input => input.Split(Environment.NewLine).ToArray(),
				data =>
				{
					var mostCommon = EncodeBools(FindMostCommon(data, false));
					var leastCommon = EncodeBools(FindMostCommon(data, true));

					return (mostCommon * leastCommon).ToString().Enumerate();
				});
		}

		private static decimal EncodeBools(string source)
		{
			return source.Aggregate(0,
				(agg, curr) =>
				{
					agg <<= 1;
					if (curr == '1')
					{
						agg |= 1;
					}

					return agg;
				});
		}

		public static string FindMostCommon(string[] input, bool inverse, int indexToMatch = 0)
		{
			var grouped = input.ToLookup(x => x[indexToMatch])
				.OrderBy(x => x.Count())
				.ThenBy(x => x.Key);

			var set = (!inverse ? grouped.Last() : grouped.First()).ToArray();

			if (set.Length == 1)
			{
				return set[0];
			}

			return FindMostCommon(set, inverse, indexToMatch + 1);
		}
	}
}

