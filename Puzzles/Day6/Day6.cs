using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day6
{
	public class Day6 : AdventDayBase
	{
		private const string InputFile = "Day6/day6.txt";

		private const string TestInput = "3,4,3,1,2";

		public Day6()
			: base(6)
		{
			AddPart(PartOne);
			AddPart(PartTwo);
		}

		// 389726
		public static AdventAssignment PartOne =>
			AdventAssignment.Build(
				InputFile,
				input => input.Split(",").Select(int.Parse),
				data => SimulateLanternFish(data, 80).Count().ToString().Enumerate());

		public static AdventAssignment PartTwo =>
			AdventAssignment.Build(
				InputFile,
				input => input.Split(",").Select(int.Parse).ToList(),
				data => SimulateLanternFishFaster(data, 256).ToString().Enumerate());


		public static IEnumerable<int> SimulateLanternFish(IEnumerable<int> data, int daysToSimulate)
		{
			if (daysToSimulate > 1)
			{
				data = SimulateLanternFish(data, daysToSimulate - 1);
			}

			foreach (var fish in data)
			{
				if (fish == 0)
				{
					yield return 6;
					yield return 8;
				}
				else
				{
					yield return fish - 1;
				}
			}
		}

		public static ulong SimulateLanternFishFaster(IEnumerable<int> data, int daysToSimulate)
		{
			var agg = Enumerable.Range(0, 9).ToDictionary(x => x, _ => (ulong) 0);
			foreach (var fish in data)
			{
				agg[fish]++;
			}

			while (daysToSimulate-- > 0)
			{
				var replicateCount = agg[0];

				agg[0] = agg[1];
				agg[1] = agg[2];
				agg[2] = agg[3];
				agg[3] = agg[4];
				agg[4] = agg[5];
				agg[5] = agg[6];
				agg[6] = agg[7] + replicateCount;
				agg[7] = agg[8];
				agg[8] = replicateCount;
			}

			var val = agg.Values.Aggregate<ulong, ulong>(0, (current, val) => current + val);
			return val;
		}

		// I started a O(1) solution but did not finish.
		//public static int FastCount(IReadOnlyList<int> data, int daysToSimulate)
		//{
		//	var (quotient, remainder) = Math.DivRem(daysToSimulate, 7);

		//	var ageAFishHasToBeToStillGetABabyInTheRemainderTime = remainder - 1;

		//	var initialFish = data.Count;
		//	var fishWithinRemainder = data.Count(x => x <= ageAFishHasToBeToStillGetABabyInTheRemainderTime);

		//	var allProducedCount = initialFish * quotient;
		//	var remainderProducedCount = fishWithinRemainder * quotient;

		//	return initialFish + allProducedCount + remainderProducedCount;
		//}
	}
}