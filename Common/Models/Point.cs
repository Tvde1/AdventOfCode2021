using System;
using System.Linq;

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

public readonly record struct Point3D(int X, int Y, int Z)
{
    public static Point3D Parse(string input)
    {
        var sp = input.Split(',');
        return new Point3D(int.Parse(sp[0]), int.Parse(sp[1]), int.Parse(sp[2]));
    }

    public static Vector3D CalculatePointTranslation(Point3D one, Point3D two)
    {
        var xDiff = one.X - two.X;
        var yDiff = one.Y - two.Y;
        var zDiff = one.Z - two.Z;

        return new Vector3D(xDiff, yDiff, zDiff);
    }

    public static Point3D RoughCenter(params Point3D[] points)
    {
        return new Point3D(points.Sum(x => x.X) / points.Length, points.Sum(x => x.Y) / points.Length, points.Sum(x => x.Z) / points.Length);
    }

    public Point3D Translate(Vector3D translation)
    {
        return this with { X = X + translation.X, Y = Y + translation.Y, Z = Z + translation.Z };
    }

    public Point3D Translate(Rotation3D rotationTranslation)
    {
        

        throw new NotImplementedException();
    }
}
