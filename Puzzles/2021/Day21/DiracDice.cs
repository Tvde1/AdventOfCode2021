using System;

namespace AdventOfCode.Puzzles._2021.Day21;

public class DiracDice
{
    private readonly DeterministicDie _die = new DeterministicDie();

    private int _playerTurn = 1;
    private int _player1Position;
    private int _player2Position;

    public DiracDice(StartPosition startPosition)
    {
        _player1Position = startPosition.Player1Position;
        _player2Position = startPosition.Player2Position;
    }

    public int Player1Score { get; private set; }

    public int Player2Score { get; private set; }

    public void Play(out int losingPlayerScore, out int timesRolled)
    {
        while (Player1Score < 1000 && Player2Score < 1000)
        {
            var roll = _die.RollThree();

            switch (_playerTurn)
            {
                case 1:
                    _player1Position += roll;
                    _player1Position = ((_player1Position - 1) % 10) + 1;

                    Player1Score += _player1Position;
                    _playerTurn = 2;
                    break;
                case 2:
                    _player2Position += roll;
                    _player2Position = ((_player2Position - 1) % 10) + 1;

                    Player2Score += _player2Position;
                    _playerTurn = 1;
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        losingPlayerScore = Math.Min(Player1Score, Player2Score);
        timesRolled = _die.TimesRolled;
    }
}



public class DeterministicDie // Isn't everything deterministic...
{
    private int _index = 0;

    public int TimesRolled { get; private set; }

    public int RollThree()
    {

        return Roll() + Roll() + Roll();
    }

    private int Roll()
    {
        TimesRolled++;

        if (_index == 100)
        {
            _index = 0;
        }

        return ++_index;
    }
}