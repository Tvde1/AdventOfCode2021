using System;
using System.Threading.Tasks;

namespace AdventOfCode.Puzzles.Day21;

public class QuantumDiracDice
{
    private static readonly int[] PossibleDiceRolls = { 1, 2, 3 };

    private static int TotalWins = 0;
    private static object _lock = new object();

    private readonly StartPosition _startPosition;

    public QuantumDiracDice(StartPosition startPosition)
    {
        _startPosition = startPosition;
    }

    public (long Player1Wins, long Player2Wins) PlayAndDoesCurrentPlayerWin()
    {
        return CalculateWins(_startPosition.Player1Position, 0, _startPosition.Player2Position, 0);
    }

    private static (long CurrentPlayerWins, long OtherPlayerWins) CalculateWins(int currentPlayerPosition, int currentPlayerScore, int otherPlayerPosition, int otherPlayerScore)
    {
        var currentWins = 0L;
        var otherWins = 0L;

        Parallel.ForEach(PossibleDiceRolls, diceRoll =>
        {
            var newPosition = (currentPlayerPosition + diceRoll - 1) % 10 + 1;
            var newScore = currentPlayerScore + newPosition;

            if (newScore >= 21)
            {
                lock (_lock)
                {
                    TotalWins++;
                }
                currentWins++;
            }
            else
            {
                var nextResult = CalculateWins(otherPlayerPosition, otherPlayerScore, newPosition, newScore);
                currentWins += nextResult.OtherPlayerWins;
                otherWins += nextResult.CurrentPlayerWins;
            }
        });

        return (currentWins, otherWins);
    }
}