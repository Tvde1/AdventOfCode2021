using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Puzzles._2021.Day21;

public class QuantumDiracDice
{
    private static readonly IReadOnlyDictionary<int, int> PossibleDiceRolls;

    private long _currentWins;
    private long _otherWins;

    static QuantumDiracDice()
    {
        var possibleDiceValues = new[] {1, 2, 3};

        PossibleDiceRolls = possibleDiceValues.SelectMany(roll1 =>
                possibleDiceValues.SelectMany(roll2 => possibleDiceValues.Select(roll3 => roll1 + roll2 + roll3)))
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => x.Count());
    }

    private readonly StartPosition _startPosition;

    public QuantumDiracDice(StartPosition startPosition)
    {
        _startPosition = startPosition;
    }

    public (long Player1Wins, long Player2Wins) Play()
    {
        CalculateWins(_startPosition.Player1Position, 0, _startPosition.Player2Position, 0, false, 1);

        return (_currentWins, _otherWins);
    }

    private void CalculateWins(int currentPlayerPosition, int currentPlayerScore, int otherPlayerPosition,
        int otherPlayerScore, bool flip, long alternateRealityCount)
    {
        foreach(var (diceValue, amountOfSplits) in PossibleDiceRolls)
        {
            var newPosition = (currentPlayerPosition + diceValue - 1) % 10 + 1;
            var newScore = currentPlayerScore + newPosition;

            var newRealityCount = alternateRealityCount * amountOfSplits;

            if (newScore >= 21)
            {
                if (flip)
                {
                    _otherWins += newRealityCount;
                }
                else
                {
                    _currentWins += newRealityCount;
                }
            }
            else
            {
                CalculateWins(otherPlayerPosition, otherPlayerScore, newPosition, newScore, !flip, newRealityCount);
            }
        }
    }
}