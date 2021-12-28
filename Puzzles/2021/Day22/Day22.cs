using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles._2021.Day22;

public class Day22 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.FromFile("Day22/day22.txt");

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"on x=10..12,y=10..12,z=10..12
on x=11..13,y=11..13,z=11..13
off x=9..11,y=9..11,z=9..11
on x=10..10,y=10..10,z=10..10");

    private static readonly AdventDataSource TestInput2 = AdventDataSource.FromRaw(@"on x=-20..26,y=-36..17,z=-47..7
on x=-20..33,y=-21..23,z=-26..28
on x=-22..28,y=-29..23,z=-38..16
on x=-46..7,y=-6..46,z=-50..-1
on x=-49..1,y=-3..46,z=-24..28
on x=2..47,y=-22..22,z=-23..27
on x=-27..23,y=-28..26,z=-21..29
on x=-39..5,y=-6..47,z=-3..44
on x=-30..21,y=-8..43,z=-13..34
on x=-22..26,y=-27..20,z=-29..19
off x=-48..-32,y=26..41,z=-47..-37
on x=-12..35,y=6..50,z=-50..-2
off x=-48..-32,y=-32..-16,z=-15..-5
on x=-18..26,y=-33..15,z=-7..46
off x=-40..-22,y=-38..-28,z=23..41
on x=-16..35,y=-41..10,z=-47..6
off x=-32..-23,y=11..30,z=-14..3
on x=-49..-5,y=-3..45,z=-29..18
off x=18..30,y=-20..-8,z=-3..13
on x=-41..9,y=-7..43,z=-33..15
on x=-54112..-39298,y=-85059..-49293,z=-27449..7877
on x=967..23432,y=45373..81175,z=27513..53682");

    public Day22()
        : base(22, AdventDayImplementation.Build(RealInput, Parse, PartOne))
    { }

    private static RebootStep[] Parse(string input) => input.Split(Environment.NewLine).Select(RebootStep.Parse).ToArray();

    private static string PartOne(RebootStep[] data)
    {
        var reactorCore = ReactorCore.Build(data);

        var statuses = reactorCore.ReactorCoreStatuses;

        var onStatuses = statuses.Where(x => x.IsOn).ToList();

        var shrunkStatuses = onStatuses.Select(x => x.Shrink(50)).WhereNotNull().ToList();

        //var sb = new StringBuilder();

        //shrunkStatuses.ForEach(x =>
        //{
        //    sb.Append("on x=");
        //    sb.Append(x.Volume.X.Start);
        //    sb.Append("..");
        //    sb.Append(x.Volume.X.End);
        //    sb.Append(",y=");
        //    sb.Append(x.Volume.Y.Start);
        //    sb.Append("..");
        //    sb.Append(x.Volume.Y.End);
        //    sb.Append(",z=");
        //    sb.Append(x.Volume.Z.Start);
        //    sb.Append("..");
        //    sb.Append(x.Volume.Z.End);
        //    sb.AppendLine();
        //});

        //Console.WriteLine(sb.ToString());

        var cubeSum = shrunkStatuses.Aggregate(0UL, (total, reactorCoreStatus) => total + reactorCoreStatus.CubeCount)
            .ToString();

        return cubeSum;
    }

    private static string PartTwo(string data) => data;
}

public readonly record struct Range1D
{
    public Range1D(int start, int end)
    {
        if (start > end)
        {
            throw new ArgumentException($"A range cannot start at {start} and end at {end}...");
        }

        Start = start;
        End = end;
    }

    public readonly int Start;
    public readonly int End;

    public ulong Distance => (ulong) Math.Abs(End - Start) + 1;

    public IEnumerable<Range1D> CalculateEncompassingRanges(Range1D other)
    {
        if (!this.IsInsideOf(other))
        {
            yield return this;
            yield break;
        }

        if (Start < other.Start)
        {
            // We start to the left

            // From start until just before other start
            yield return new Range1D(Start, other.Start - 1);

            if (End <= other.End)
            {
                // If we stop in(side)
                yield return new Range1D(other.Start, End);
            }
            else
            {
                // If we stop outside
                yield return new Range1D(other.Start, other.End);
                yield return new Range1D(other.End + 1, End);
            }

            // Done
        }
        else
        {
            // We start inside

            if (End <= other.End)
            {
                // If we stop in(side)
                yield return new Range1D(Start, End);
            }
            else
            {
                // We stop outside
                yield return new Range1D(Start, other.End);
                yield return new Range1D(other.End + 1, End);
            }

            // Done
        }
    }

    public bool IsInsideOf(Range1D other)
    {
        return Start <= other.End && End >= other.Start;
    }

    public Range1D? Shrink(int maxValue)
    {
        var newStart = Math.Max(Math.Min(Start, maxValue), -maxValue);
        var newEnd = Math.Max(Math.Min(End, maxValue), -maxValue);

        if (newStart == Start && newEnd == End)
        {
            return this;
        }

        return new Range1D(newStart, newEnd);
    }
}

public record struct CuboidVolume(Range1D X, Range1D Y, Range1D Z)
{
    public ulong Volume => X.Distance * Y.Distance * Z.Distance;

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
    /// <param name="other">The <see cref="CuboidVolume"/> to split the current <see cref="CuboidVolume"/> for.</param>
    /// <returns></returns>
    public IEnumerable<CuboidVolume> SplitSelf(CuboidVolume other)
    {
        var xRanges = this.X.CalculateEncompassingRanges(other.X);
        var yRanges = this.Y.CalculateEncompassingRanges(other.Y).ToArray();
        var zRanges = this.Z.CalculateEncompassingRanges(other.Z).ToArray();

        return xRanges.SelectMany(xRange =>
            yRanges.SelectMany(yRange =>
                zRanges.Select(zRange => new CuboidVolume(xRange, yRange, zRange))));
    }

    public bool IsInsideOf(CuboidVolume other)
    {
        return X.IsInsideOf(other.X) && Y.IsInsideOf(other.Y) && Z.IsInsideOf(other.Z);
    }

    public CuboidVolume? Shrink(int maxValue)
    {
        var newXRange = X.Shrink(maxValue);
        var newYRange = Y.Shrink(maxValue);
        var newZRange = Z.Shrink(maxValue);

        if (!newXRange.HasValue || !newYRange.HasValue || !newZRange.HasValue)
        {
            return null;
        }

        return new CuboidVolume(newXRange.Value, newYRange.Value, newZRange.Value);
    }
}

public record struct RebootStep(bool turnOn, CuboidVolume Volume)
{
    private static readonly Regex ParseRegex = new(@"(on|off) x=(\-?\d+)\.\.(\-?\d+),y=(\-?\d+)\.\.(\-?\d+),z=(\-?\d+)\.\.(\-?\d+)");

    public static RebootStep Parse(string input)
    {
        var match = ParseRegex.Match(input);

        var turnOn = match.Groups[1].Value == "on";

        var xRange = new Range1D(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
        var yRange = new Range1D(int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value));
        var zRange = new Range1D(int.Parse(match.Groups[6].Value), int.Parse(match.Groups[7].Value));

        return new RebootStep(turnOn, new CuboidVolume(xRange, yRange, zRange));
    }
}

public record struct ReactorCoreStatus(bool IsOn, CuboidVolume Volume)
{
    public ulong CubeCount => Volume.Volume;

    public IEnumerable<ReactorCoreStatus> Split(RebootStep rebootStep)
    {
        var isOn = IsOn;
        return Volume.SplitSelf(rebootStep.Volume).Select(newRange => new ReactorCoreStatus(isOn, newRange));
    }

    public ReactorCoreStatus? Shrink(int maxValue)
    {
        var newRange = Volume.Shrink(maxValue);

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
            var newStatuses = new List<ReactorCoreStatus>();
            foreach (var splitsThatDoNotOverlapNewRange in reactorCoreStatuses
                         .Select(existingStatus => existingStatus.Split(rebootStep).ToList()).Select(split =>
                             split.Where(x => !x.Volume.IsInsideOf(rebootStep.Volume)).ToList()))
            {
                newStatuses.AddRange(splitsThatDoNotOverlapNewRange);
            }

            reactorCoreStatuses = newStatuses;
            reactorCoreStatuses.Add(new ReactorCoreStatus(rebootStep.turnOn, rebootStep.Volume));
        }

        return new ReactorCore(reactorCoreStatuses);
    }
}