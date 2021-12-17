using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day02;

using Day2Data = IEnumerable<Command>;

public readonly record struct Command(Command.DirectionType Direction, int Count)
{
    public enum DirectionType : byte
    {
        Up,
        Down,
        Forward
    }

    public static Command Parse(string input)
    {
        var split = input.Split(StringConstants.Space);
        return new Command(split[0] switch
        {
            "up" => DirectionType.Up,
            "down" => DirectionType.Down,
            "forward" => DirectionType.Forward,
            _ => throw new ArgumentOutOfRangeException(nameof(Direction)),
        }, int.Parse(split[1]));
    }
}

public class Day2 : AdventDay<Day2Data>
{

    private const string InputFile = "Day2/day2.txt";

    public Day2()
        : base(2, AdventDataSource.FromFile(InputFile), Parse, PartOne, PartTwo)
    { }

    private static Day2Data Parse(string input) => input.Split(Environment.NewLine).Select(Command.Parse);

    public readonly record struct State
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

    private static string PartOne(Day2Data data) => data.Aggregate(
                State.Empty,
                (state, command) => command switch
                {
                    { Direction: Command.DirectionType.Up } => state with
                    {
                        Depth = state.Depth - command.Count
                    },
                    { Direction: Command.DirectionType.Down } => state with
                    {
                        Depth = state.Depth + command.Count
                    },
                    { Direction: Command.DirectionType.Forward } => state with
                    {
                        HorizontalPosition = state.HorizontalPosition + command.Count
                    }
                }).ComputeResult().ToString();

    private static string PartTwo(Day2Data data) => data.Aggregate(
                State.Empty,
                (state, command) => command switch
                {
                    { Direction: Command.DirectionType.Up } => state with
                    {
                        Aim = state.Aim - command.Count
                    },
                    { Direction: Command.DirectionType.Down } => state with
                    {
                        Aim = state.Aim + command.Count
                    },
                    { Direction: Command.DirectionType.Forward } => state with
                    {
                        HorizontalPosition = state.HorizontalPosition + command.Count,
                        Depth = state.Depth + state.Aim * command.Count
                    }
                }).ComputeResult().ToString();
}