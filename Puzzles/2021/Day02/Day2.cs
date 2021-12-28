using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles._2021.Day02;

public class Day2 : AdventDay
{
    private const string InputFile = "Day02/day2.txt";

    public Day2()
        : base(AdventDayImplementation.Build(AdventDataSource.ForThisDay(), Parse, PartOne, PartTwo))
    { }

    private static IEnumerable<Command> Parse(string input) => input.Split(Environment.NewLine).Select(Command.Parse);

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

    private static string PartOne(IEnumerable<Command> data) => data.Aggregate(
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
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
                }).ComputeResult().ToString();

    private static string PartTwo(IEnumerable<Command> data) => data.Aggregate(
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
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
                }).ComputeResult().ToString();
}