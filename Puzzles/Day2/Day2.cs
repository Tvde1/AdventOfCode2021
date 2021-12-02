using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day2
{
	public class Day2 : AdventDayBase
	{
		private const string InputFile = "Day2/day2.txt";

		private enum Direction : byte
		{
			Up,
			Down,
			Forward,
		}

		private record Command(Direction Direction, int Count);

		private record State(int Depth, int HorizontalPosition);

		private record State2(int Depth, int HorizontalPosition, int Aim);

		public Day2()
			: base(1)
		{
			AddPart(BuildPartOne());
			AddPart(BuildPartTwo());
		}

		public static AdventAssignment BuildPartOne()
		{
			return AdventAssignment.Build(
				1,
				InputFile,
				input => input.Split(Environment.NewLine).Select(x => x.Split(" ")).Select(ParseCommand),
				data =>
				{
					var (depth, horizontalPosition) = data.Aggregate(
						new State(0, 0),
						(state, command) => command switch
						{
							{Direction: Direction.Up} => state with
							{
								Depth = state.Depth - command.Count
							},
							{Direction: Direction.Down} => state with
							{
								Depth = state.Depth + command.Count
							},
							{Direction: Direction.Forward} => state with
							{
								HorizontalPosition = state.HorizontalPosition + command.Count
							},
						});
					return (depth * horizontalPosition).ToString().Enumerate();
				});
		}

		public static AdventAssignment BuildPartTwo()
		{
			return AdventAssignment.Build(
				2,
				InputFile,
				input => input.Split(Environment.NewLine).Select(x => x.Split(" ")).Select(ParseCommand),
				data =>
				{
					var (depth, horizontalPosition, _) = data.Aggregate(
						new State2(0, 0, 0),
						(state, command) => command switch
						{
							{Direction: Direction.Up} => state with
							{
								Aim = state.Aim - command.Count,
							},
							{Direction: Direction.Down} => state with
							{
								Aim = state.Aim + command.Count,
							},
							{Direction: Direction.Forward} => state with
							{
								HorizontalPosition = state.HorizontalPosition + command.Count,
								Depth = state.Depth + (state.Aim * command.Count),
							},
						});
					return (depth * horizontalPosition).ToString().Enumerate();
				});
		}

		private static Command ParseCommand(string[] input)
		{
			return new Command(input[0] switch
			{
				"up" => Direction.Up,
				"down" => Direction.Down,
				"forward" => Direction.Forward,
			}, int.Parse(input[1]));
		}
	}
}

