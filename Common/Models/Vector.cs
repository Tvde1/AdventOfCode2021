using System;

namespace AdventOfCode.Common.Models;

public readonly record struct Vector2D(int X, int Y);

public readonly record struct Vector3D(int X, int Y, int Z)
{
    public static Vector3D None = new(0, 0, 0);

    public double PythagoreanDistance()
    {
        return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
    }

    public Vector3D Apply(Vector3D other)
    {
        return this with { X = X * other.X, Y = Y * other.Y, Z = Z * other.Z };
    }

    public Vector3D Merge(Vector3D other)
    {
        return this with { X = X + other.X, Y = Y + other.Y, Z = Z + other.Z };
    }

    public static Vector3D operator +(Vector3D left, Vector3D right)
    {
        return left.Apply(right);
    }

    public static Vector3D Parse(string input)
    {
        var sp = input.Split(',');
        return new Vector3D(int.Parse(sp[0]), int.Parse(sp[1]), int.Parse(sp[2]));
    }
}
