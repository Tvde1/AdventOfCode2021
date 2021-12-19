using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Common.Models;

public readonly record struct Rotation3D(int X, int Y, int Z)
{
    public static readonly IReadOnlyCollection<Rotation3D> AllRotationTranslations = CalculateAllRotationTranslations().ToArray();

    private static IEnumerable<Rotation3D> CalculateAllRotationTranslations()
    {
        var angles = new[] { 0, 90, 180, 270, };

        foreach (var xAngle in angles)
            foreach (var yAngle in angles)
                foreach (var zAngle in angles)
                    yield return new(xAngle, yAngle, zAngle);
    }
}