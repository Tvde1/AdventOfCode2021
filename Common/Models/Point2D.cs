using System;

namespace AdventOfCode.Common.Models;

public readonly record struct Point2D(int X, int Y) : IComparable<Point2D>
{
    public static Point2D Parse(string input)
    {
        var sp = input.Split(',');
        return new Point2D(int.Parse(sp[0]), int.Parse(sp[1]));
    }

    public int CompareTo(Point2D other)
    {
        var xComparison = X.CompareTo(other.X);
        return xComparison != 0 ? xComparison : Y.CompareTo(other.Y);
    }
}