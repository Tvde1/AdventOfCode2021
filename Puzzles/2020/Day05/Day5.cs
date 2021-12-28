using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Puzzles._2020.Day05;

public class Day5 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"BFFFBBFRRR
BBFFBBFRLL
FFFBBBFRRR");

    public Day5()
        : base(AdventDayImplementation.Build(RealInput, Parse, PartOne, PartTwo))
    { }

    private static string[] Parse(string input) => input.Split(Environment.NewLine);

    private static string PartOne(string[] data)
    {
        var seats = data.Select(ParseSeat);

        var highest = seats.OrderByDescending(x => x).First();

        return highest.ToString();
    }

    private static string PartTwo(string[] data)
    {
        var seats = data.Select(ParseSeat);

        var remainingSeats = Enumerable.Range(46, 992 - 46).Except(seats).ToList();

        var mySeat = remainingSeats.Single();

        return mySeat.ToString();
    }

    private static int ParseSeat(string input)
    {
        input = input.Replace("B", "1");
        input = input.Replace("F", "0");
        input = input.Replace("R", "1");
        input = input.Replace("L", "0");

        return Convert.ToInt32(input, 2);
    }

    private static int CalculateSeatId((int Row, int Column) seat) => seat.Row * 8 + seat.Column;
}