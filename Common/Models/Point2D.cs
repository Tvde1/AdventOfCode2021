namespace AdventOfCode.Common.Models;

public readonly record struct Point2D(int X, int Y)
{
    public static Point2D Parse(string input)
    {
        var sp = input.Split(',');
        return new Point2D(int.Parse(sp[0]), int.Parse(sp[1]));
    }
}