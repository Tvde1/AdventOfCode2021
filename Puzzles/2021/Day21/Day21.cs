using System;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles._2021.Day21;

public class Day21 : AdventDay
{
    private static readonly AdventDataSource PuzzleInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"Player 1 starting position: 4
Player 2 starting position: 8");

    public Day21()
        : base(AdventDayImplementation.Build(PuzzleInput, StartPosition.Parse, PartOne, PartTwo))
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

        var result = quantumGame.Play();

        return $"Player 1 wins {result.Player1Wins} games and \nPlayer 2 wins {result.Player2Wins} games.";
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