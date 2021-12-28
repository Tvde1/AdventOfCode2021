using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles._2021.Day04;

public class Day4 : AdventDay
{
    private const string InputFile = "Day04/day4.txt";

    public Day4()
        : base(4, AdventDayImplementation.Build(AdventDataSource.FromFile(InputFile), Parse, PartOne, PartTwo))
    { }

    public class BoardValue
    {
        public BoardValue(int number, bool isChecked, Point2D location)
        {
            Number = number;
            IsChecked = isChecked;
            Location = location;
        }

        public int Number { get; }
        public bool IsChecked { get; set; }
        public Point2D Location { get; }
    }

    public class Board
    {
        private static readonly Point2D[][] WinPositions = CalcWinPositions().ToArray();

        private readonly Dictionary<Point2D, BoardValue> _valueByLocation;
        private readonly Dictionary<int, BoardValue> _valueByValue;

        public Board(IReadOnlyCollection<BoardValue> values)
        {
            _valueByLocation = values.ToDictionary(x => x.Location);
            _valueByValue = values.ToDictionary(x => x.Number);
        }

        public int SumOfAllUnmarked => _valueByValue.Values.Where(x => !x.IsChecked).Sum(x => x.Number);

        private static IEnumerable<Point2D[]> CalcWinPositions()
        {
            var range = Enumerable.Range(0, 5).ToArray();

            var verticals = range.Select(x => range.Select(y => new Point2D(x, y)).ToArray());
            var horizontals = range.Select(y => range.Select(x => new Point2D(x, y)).ToArray());

            var diag1 = range.Select(x => new Point2D(x, x)).ToArray();
            var diag2 = range.Select(x => new Point2D(x, 4 - x)).ToArray();
            return verticals.Concat(horizontals).Append(diag1).Append(diag2);
        }

        public Board? Register(int num)
        {
            if (!_valueByValue.TryGetValue(num, out var v)) return null;

            v.IsChecked = true;

            return CalculateWin();
        }

        private Board? CalculateWin()
        {
            return WinPositions.Select(arr => arr.Select(pos => _valueByLocation[pos].IsChecked))
                .Any(arr => arr.All(x => x))
                ? this
                : null;
        }
    }

    private static (int[] Numbers, List<Board> Boards) Parse(string input)
    {
        var split = input.Split(Environment.NewLine);

        var numbers = split[0].Split(",").Select(int.Parse).ToArray();

        var completedBoards = new List<Board>();

        var currentBoardValues = new List<BoardValue>();
        var currentY = 0;
        foreach (var s in split.Skip(2))
        {
            if (string.IsNullOrEmpty(s))
            {
                currentY = 0;
                completedBoards.Add(new Board(currentBoardValues));
                currentBoardValues.Clear();
                continue;
            }

            currentBoardValues.AddRange(s.Replace(StringConstants.DoubleSpace, StringConstants.Space).Trim()
                .Split(StringConstants.Space).Select((num, x) =>
                    new BoardValue(int.Parse(num), false, new Point2D(x, currentY))));
            currentY++;
        }

        completedBoards.Add(new Board(currentBoardValues));

        return (Numbers: numbers, Boards: completedBoards);
    }

    // 2743844
    private static string PartOne((int[] Numbers, List<Board> Boards) data)
    {
        foreach (var dataNumber in data.Numbers)
        {
            var winningBoard = data.Boards.FirstOrDefault(x => x.Register(dataNumber) is not null);
            if (winningBoard is null) continue;

            return (winningBoard.SumOfAllUnmarked * dataNumber).ToString();
        }

        return "No found";
    }

    private static string PartTwo((int[] Numbers, List<Board> Boards) data)
    {
        foreach (var dataNumber in data.Numbers)
        {
            var winningBoard = data.Boards.Where(x => x.Register(dataNumber) is not null).ToList();
            winningBoard.ForEach(x => data.Boards.Remove(x));

            if (data.Boards.Count == 0)
            {
                Debug.Assert(winningBoard.Count == 1);
                return (winningBoard.First().SumOfAllUnmarked * dataNumber).ToString();
            }
        }

        return "No found";
    }
}