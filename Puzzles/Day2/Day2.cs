using System;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day2
{
	public class Day2 : AdventDayBase
	{
		private const string InputFile = "Day2/day2.txt";

		public Day2()
			: base(2)
		{
			AddPart(BuildPartOne());
			AddPart(BuildPartTwo());
		}

		public static AdventAssignment BuildPartOne()
		{
			return AdventAssignment.Build(
				InputFile,
				input => input.Split(Environment.NewLine).Select(Command.Parse),
				data => data.Aggregate(
					State.Empty,
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
						}
					}).ComputeResult().ToString().Enumerate());
		}

		public static AdventAssignment BuildPartTwo()
		{
			return AdventAssignment.Build(
				InputFile,
				input => input.Split(Environment.NewLine).Select(Command.Parse),
				data => data.Aggregate(
					State.Empty,
					(state, command) => command switch
					{
						{Direction: Direction.Up} => state with
						{
							Aim = state.Aim - command.Count
						},
						{Direction: Direction.Down} => state with
						{
							Aim = state.Aim + command.Count
						},
						{Direction: Direction.Forward} => state with
						{
							HorizontalPosition = state.HorizontalPosition + command.Count,
							Depth = state.Depth + state.Aim * command.Count
						}
					}).ComputeResult().ToString().Enumerate());
		}

		private enum Direction : byte
		{
			Up,
			Down,
			Forward
		}

		private readonly record struct Command(Direction Direction, int Count)
		{
			public static Command Parse(string input)
			{
				var split = input.Split(StringConstants.Space);
				return new Command(split[0] switch
				{
					"up" => Direction.Up,
					"down" => Direction.Down,
					"forward" => Direction.Forward
				}, int.Parse(split[1]));
			}
		}

		private readonly record struct State
		{
			private State(int depth, int horizontalPosition, int aim)
			{
				Depth = depth;
				HorizontalPosition = horizontalPosition;
				Aim = aim;
			}

			public int Depth { get; init; }
			public int HorizontalPosition { get; init; }
			public int Aim { get; init; }

			public static State Empty => new(0, 0, 0);

			public int ComputeResult()
			{
				return Depth * HorizontalPosition;
			}
		}
	}
}