using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day22;

public class Day22 : AdventDay
{
    private static AdventDataSource RealInput = AdventDataSource.FromFile("Day22/day22.txt");

    private static AdventDataSource TestInput = AdventDataSource.FromRaw(@"on x=10..12,y=10..12,z=10..12
on x=11..13,y=11..13,z=11..13
off x=9..11,y=9..11,z=9..11
on x=10..10,y=10..10,z=10..10");

    public Day22()
        : base(22, AdventDayImplementation.Build(TestInput, Parse, PartOne))
    { }

    private static IEnumerable<RebootStep> Parse(string input) => input.Split(Environment.NewLine).Select(RebootStep.Parse);

    private static string PartOne(IEnumerable<RebootStep> data)
    {
        var reactorCore = ReactorCore.Build(data);

        return reactorCore.ReactorCoreStatuses
            .Where(x => x.IsOn)
            .Select(x => x.Shrink(50))
            .WhereNotNull()
            .Sum(x => x.CubeCount).ToString();
    }

    private static string PartTwo(string data) => data;
}

public record struct Range1D
{
    public Range1D(int start, int end)
    {
        if (start > end)
        {
            throw new ArgumentException("Invalid range.");
        }

        Start = start;
        End = end;
    }

    public readonly int Start;
    public readonly int End;

    public int Distance => Math.Abs(End - Start);

    public int? IntersectsFirstAt(Range1D other)
    {
        if (!Overlaps(other))
        {
            return null;
        }

        return other.Start < Start ? Start : other.Start;
    }

    public bool Overlaps(Range1D other)
    {
        return other.Start <= End && other.End >= Start;
    }

    public Range1D? Shrink(int maxValue)
    {
        if (Start > maxValue)
        {
            return null;
        }

        if (End <= maxValue)
        {
            return this;
        }

        return new Range1D(Start, maxValue);
    }
}

public record struct CuboidRange(Range1D X, Range1D Y, Range1D Z)
{
    public int Volume => X.Distance * Y.Distance * Z.Distance;

    /// <summary>
    /// Splits the current range in parts such that the other range fits perfectly.
    /// For example:
    /// Existing range : A
    /// New range: B
    /// Overlap: X
    ///
    /// AAAAAAAA
    /// AAAAABBBB
    /// AAAAABBBB
    ///      BBBB
    ///
    /// Results in
    ///
    /// 11111222
    /// 11111BBBB
    /// 11111BBBB
    ///      BBBB
    /// </summary>
    /// <param name="other">The <see cref="CuboidRange"/> to split the current <see cref="CuboidRange"/> for.</param>
    /// <returns></returns>
    public List<CuboidRange> SplitSelf(CuboidRange other)
    {
        var ranges = new List<CuboidRange>();

        if (!other.Overlaps(this))
        {
           ranges.Add(this);
           return ranges;
        }

        var self = this;

        while (other.Overlaps(self))
        {
            var xSplit = self.X.IntersectsFirstAt(other.X);
            var ySplit = self.Y.IntersectsFirstAt(other.Y);
            var zSplit = self.Z.IntersectsFirstAt(other.Z);

            var newCuboidXRange = xSplit.HasValue ? new Range1D(self.X.Start, xSplit.Value) : self.X;
            var newCuboidYRange = ySplit.HasValue ? new Range1D(self.Y.Start, self.Y.End) : self.Y;
            var newCuboidZRange = zSplit.HasValue ? new Range1D(self.Z.Start, self.Z.End) : self.Z;

            ranges.Add(new CuboidRange(newCuboidXRange, newCuboidYRange, newCuboidZRange));

            var recalculatedXRange = xSplit.HasValue ? new Range1D(xSplit.Value, self.X.End) : self.X;
            var recalculatedYRange = ySplit.HasValue ? new Range1D(ySplit.Value, self.Y.End) : self.Y;
            var recalculatedZRange = zSplit.HasValue ? new Range1D(zSplit.Value, self.Z.End) : self.Z;

            self = new CuboidRange(recalculatedXRange, recalculatedYRange, recalculatedZRange);
        }

        return ranges;
    }

    public bool Overlaps(CuboidRange other)
    {
        return X.Overlaps(other.X) || Y.Overlaps(other.Y) || Z.Overlaps(other.Z);
    }

    public CuboidRange? Shrink(int maxValue)
    {
        var newXRange = X.Shrink(maxValue);
        var newYRange = Y.Shrink(maxValue);
        var newZRange = Z.Shrink(maxValue);

        if (!newXRange.HasValue || !newYRange.HasValue || !newZRange.HasValue)
        {
            return null;
        }

        return new CuboidRange(newXRange.Value, newYRange.Value, newZRange.Value);
    }
}

public record struct RebootStep(bool turnOn, CuboidRange Range)
{
    private static readonly Regex ParseRegex = new(@"(on|off) x=([\-\d]+)\.\.([\-\d]+),y=x=([\-\d]+)\.\.([\-\d]+),z=([\-\d]+)\.\.([\-\d]+)");

    public static RebootStep Parse(string input)
    {
        var match = ParseRegex.Match(input);

        var turnOn = match.Groups[1].Value == "on";

        var xRange = new Range1D(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
        var yRange = new Range1D(int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value));
        var zRange = new Range1D(int.Parse(match.Groups[6].Value), int.Parse(match.Groups[7].Value));

        return new RebootStep(turnOn, new CuboidRange(xRange, yRange, zRange));
    }
}

public record struct ReactorCoreStatus(bool IsOn, CuboidRange Range)
{
    public int CubeCount => Range.Volume;

    public IEnumerable<ReactorCoreStatus> Split(RebootStep rebootStep)
    {
        foreach (var newRange in Range.SplitSelf(rebootStep.Range))
        {
            yield return new ReactorCoreStatus(IsOn, newRange);
        }
    }

    public ReactorCoreStatus? Shrink(int maxValue)
    {
        var newRange = Range.Shrink(maxValue);

        if (!newRange.HasValue)
        {
            return null;
        }

        return new ReactorCoreStatus(IsOn, newRange.Value);
    }
}

public class ReactorCore
{
    private ReactorCore(IReadOnlyList<ReactorCoreStatus> reactorCoreStatuses)
    {
        ReactorCoreStatuses = reactorCoreStatuses;
    }

    public IReadOnlyList<ReactorCoreStatus> ReactorCoreStatuses { get; }

    public static ReactorCore Build(IEnumerable<RebootStep> rebootSteps)
    {
        var reactorCoreStatuses = new List<ReactorCoreStatus>();

        foreach (var rebootStep in rebootSteps)
        {
            reactorCoreStatuses = reactorCoreStatuses.SelectMany(x => x.Split(rebootStep).Where(x => !x.Range.Overlaps(rebootStep.Range))).ToList();
            reactorCoreStatuses.Add(new ReactorCoreStatus(rebootStep.turnOn, rebootStep.Range));
        }

        return new ReactorCore(reactorCoreStatuses);
    }
}