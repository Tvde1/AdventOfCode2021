using System;

namespace AdventOfCode.Common.Models;

public readonly record struct Vector2D(int X, int Y);

public readonly record struct Vector3D(int X, int Y, int Z)
{
    public double PythagoreanDistance()
    {
        return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
    }
}
