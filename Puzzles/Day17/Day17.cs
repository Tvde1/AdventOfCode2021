using System;
using System.Collections.Generic;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles.Day17;

public class Day17 : AdventDayBase
{
    private const string InputFile = "Day17/day17.txt";

    private const string TestInput = @"target area: x=20..30, y=-10..-5";

    public Day17()
        : base(17)
    {
        AddPart(PartOne);
        AddPart(PartTwo);
    }

    public static AdventAssignment PartOne =>
        AdventAssignment.Build(
            InputFile,
            input => input,
            data => data);

    public static AdventAssignment PartTwo =>
        AdventAssignment.Build(
            InputFile,
            input => input,
            data => data);

    private readonly record struct FiringError(bool IsHit, Vector2 Distance);

    private readonly record struct TargetArea(int MinX, int MaxX, int MinY, int MaxY)
    {
        public FiringError GetFiringError(Point2D target)
        {
            var isInX = target.X >= MinX && target.X <= MaxX;
            var isInY = target.Y >= MinY && target.Y <= MaxY;

            if (isInX && isInY)
            {
                return new FiringError(true, new Vector2(0, 0));
            }

            var toLeftOfX = MinX - target.X;
            var rightOfX = target.X - MaxX;

            var aboveY = target.Y - MaxY;
            var belowY = MinY - target.Y;

            var firingErrorX = 0;
            if (toLeftOfX > 0)
            {
                firingErrorX = -toLeftOfX;
            }
            else
            {
                firingErrorX = rightOfX;
            }

            var firingErrorY = 0;
            if (belowY > 0)
            {
                firingErrorY = -belowY;
            }
            else
            {
                firingErrorY = aboveY;
            }

            return new FiringError(false, new Vector2(firingErrorX, firingErrorY));
        }
    }

    private readonly record struct Probe(Point2D Point, Vector2 Velocity);

    private Probe CalculateProbeStep(Probe probe)
    {
        var newX = probe.Point.X + probe.Velocity.X;
        var newY = probe.Point.Y + probe.Velocity.Y;

        var newXVelocity = probe.Velocity.X - 1;
        var newYVelocity = probe.Velocity.Y switch
        {
            > 0 => probe.Velocity.Y - 1,
            < 0 => probe.Velocity.Y - 1,
            0 => 0,
        };

        return new Probe(new Point2D(newX, newY), new Vector2(newXVelocity, newYVelocity));
    }
}