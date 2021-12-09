using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day4
{
	public class Day4 : AdventDayBase
	{
		private const string InputFile = "Day4/day4.txt";

		public Day4()
			: base(4)
		{
			AddPart(PartOne);
			AddPart(PartTwo);
		}

		// 2743844
		private static AdventAssignment PartOne =
			AdventAssignment.Build(
				InputFile,
				input =>
				{
					var split = input.Split(Environment.NewLine)
						.ToArray();

					var numbers = split[0].Split(",").Select(int.Parse).ToArray();

					var completedBoards = new List<Board>();

					var currentBoardValues = new List<BoardValue>();
					int currentY = 0;
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
								new BoardValue(int.Parse(num), false, new Vector2(x, currentY))));
						currentY++;
					}

					completedBoards.Add(new Board(currentBoardValues));

					return (Numbers: numbers, Boards: completedBoards);
				},
				data =>
				{
					foreach (var dataNumber in data.Numbers)
					{
						var winningBoard = data.Boards.FirstOrDefault(x => x.Register(dataNumber) is not null);
						if (winningBoard is null)
						{
							continue;
						}

						return (winningBoard.SumOfAllUnmarked * dataNumber);
					}

					return "No found".Enumerate();
				});

		private static AdventAssignment PartTwo = AdventAssignment.Build(
			InputFile,
			input =>
			{
				var split = input.Split(Environment.NewLine)
					.ToArray();

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
							new BoardValue(int.Parse(num), false, new Vector2(x, currentY))));
					currentY++;
				}

				completedBoards.Add(new Board(currentBoardValues));

				return (Numbers: numbers, Boards: completedBoards);
			},
			data =>
			{
				foreach (var dataNumber in data.Numbers)
				{
					var winningBoard = data.Boards.Where(x => x.Register(dataNumber) is not null).ToList();
					winningBoard.ForEach(x => data.Boards.Remove(x));

					if (data.Boards.Count == 0)
					{
						return (winningBoard.First().SumOfAllUnmarked * dataNumber);
					}

				}

				return "No found".Enumerate();
			});

		private readonly record struct Vector2(int X, int Y);

		private class BoardValue
		{
			public int Number { get; set; }
			public bool IsChecked { get; set; }
			public Vector2 Location { get; set; }

			public BoardValue(int number, bool isChecked, Vector2 location)
			{
				Number = number;
				IsChecked = isChecked;
				Location = location;
			}
		}

		private class Board
		{
			private static Vector2[][] WinPositions = CalcWinPositions().ToArray();
			public int SumOfAllUnmarked => _valueByValue.Values.Where(x => !x.IsChecked).Sum(x => x.Number);

			private static IEnumerable<Vector2[]> CalcWinPositions()
			{
				var range = Enumerable.Range(0, 5);

				var verticals = range.Select(x => range.Select(y => new Vector2(x, y)).ToArray());
				var horizontals = range.Select(y => range.Select(x => new Vector2(x, y)).ToArray());

				var diag1 = range.Select(x => new Vector2(x, x)).ToArray();
				var diag2 = range.Select(x => new Vector2(x, 4 - x)).ToArray();
				return verticals.Concat(horizontals).Append(diag1).Append(diag2);
			}

			private readonly Dictionary<Vector2, BoardValue> _valueByLocation;
			private readonly Dictionary<int, BoardValue> _valueByValue;

			public Board(IReadOnlyCollection<BoardValue> values)
			{
				_valueByLocation = values.ToDictionary(x => x.Location);
				_valueByValue = values.ToDictionary(x => x.Number);
			}

			public Board? Register(int num)
			{
				if (!_valueByValue.TryGetValue(num, out var v))
				{
					return null;
				}

				v.IsChecked = true;

				return CalculateWin();
			}

			private Board? CalculateWin()
			{
				return WinPositions.Select(arr => arr.Select(pos => _valueByLocation[pos].IsChecked))
					.Any(arr => arr.All(x => x)) ? this : null;
			}
		}
	}
}