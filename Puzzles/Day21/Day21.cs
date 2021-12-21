using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day21;

public class Day21 : AdventDay
{
    private static AdventDataSource PuzzleInput = AdventDataSource.FromFile("Day21/day21.txt");

    private static AdventDataSource TestInput = AdventDataSource.FromRaw(@"Player 1 starting position: 4
Player 2 starting position: 8");

    public Day21()
        : base(21, AdventDayImplementation.Build(TestInput, StartPosition.Parse, PartOne, PartTwo))
    { }

    private static string PartOne(StartPosition data)
    {
        var game = new DiracDice(data);

        game.Play(out var losingPlayerScore, out var timesRolled);

        return (losingPlayerScore * timesRolled).ToString();
    }

    private static string PartTwo(StartPosition data)
    {
        var quantumGame = new QuantumDiracDice(data);

        var result = quantumGame.PlayAndDoesCurrentPlayerWin();

        return Math.Max(result.Player1Wins, result.Player2Wins).ToString();
    }
}

public readonly record struct StartPosition(int Player1Position, int Player2Position)
{
    public static StartPosition Parse(string input)
    {
        var s = input.Split(Environment.NewLine);

        return new StartPosition(s[0][^1] - '0', s[1][^1] - '0');
    }
}