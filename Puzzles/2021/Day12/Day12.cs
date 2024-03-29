﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles._2021.Day12;

public class Day12 : AdventDay
{
    private const string CaveStart = "start";
    private const string CaveEnd = "end";

    private const string TestInput = @"start-A
start-b
A-c
A-b
b-d
A-end
b-end";

    private const string TestInput2 = @"dc-end
HN-start
start-kj
dc-start
dc-HN
LN-dc
HN-end
kj-sa
kj-HN
kj-dc";

    public Day12()
        : base(AdventDayImplementation.Build(AdventDataSource.ForThisDay(), Parse, PartOne, PartTwo))
    { }

    private static Dictionary<string, Cave> Parse(string input) => GetAllCavesFull(input.Split(Environment.NewLine));

    private static string PartOne(Dictionary<string, Cave> data) => GetFullFromCurrentToEndPaths(data, CaveStart, CaveEnd).Count().ToString();

    private static string PartTwo(Dictionary<string, Cave> data) =>  data.Values
                .Where(cave => cave.CaveType == CaveType.Small)
                .SelectMany(c => GetFullFromCurrentToEndPaths2(data, CaveStart, CaveEnd, c.Name))
                .Distinct().Count().ToString();

    private static ILookup<string, string> GetAllCaveConnections(IEnumerable<string> inputs)
    {
        var split = inputs.Select(x => x.Split('-')).ToList();

        return split
            .Union(split.Select(x => x.Reverse().ToArray()))
            .ToLookup(x => x[0], x => x[1]);
    }

    private static IEnumerable<string> GetFullFromCurrentToEndPaths(Dictionary<string, Cave> allPaths, string currentCave, string endCave,
        List<string>? currentRoute = null,
        HashSet<string>? visitedSmallCaves = null)
    {
        currentRoute ??= new List<string>();
        visitedSmallCaves ??= new HashSet<string>();

        currentRoute.Add(currentCave);
        if (currentCave.All(char.IsLower))
        {
            visitedSmallCaves.Add(currentCave);
        }

        if (currentCave == endCave)
        {
            yield return string.Join(",", currentRoute);
            yield break;
        }

        var cave = allPaths[currentCave];
        foreach (var subCave in cave.Connections)
        {
            if (subCave.All(char.IsLower) && visitedSmallCaves.Contains(subCave))
            {
                continue;
            }

            foreach (var subPath in GetFullFromCurrentToEndPaths(allPaths, subCave, endCave, new List<string>(currentRoute), new HashSet<string>(visitedSmallCaves)))
            {
                yield return subPath;
            }
        }
    }

    private enum CaveType
    {
        Big,
        Small,
        StartEnd,
    }

    private readonly record struct Cave(string Name, ICollection<string> Connections, CaveType CaveType);

    private static Dictionary<string, Cave> GetAllCavesFull(string[] input)
    {
        var dict = new Dictionary<string, Cave>();
        var connections = GetAllCaveConnections(input);

        foreach (var cave in connections)
        {
            var caveType = CaveType.Big;

            if (cave.Key.All(char.IsLower))
            {
                if (cave.Key is CaveEnd or CaveStart)
                {
                    caveType = CaveType.StartEnd;
                }
                else
                {
                    caveType = CaveType.Small;
                }
            }

            var caveO = new Cave
            {
                Name = cave.Key,
                Connections = cave.OrderBy(x => x).ToArray(),
                CaveType = caveType,
            };

            dict.Add(cave.Key, caveO);
        }

        return dict;
    }

    private static IEnumerable<string> GetFullFromCurrentToEndPaths2(IDictionary<string, Cave> allPaths, string currentCaveName, string endCave,
        string specialSmallCave,
        List<string>? currentRoute = null,
        Dictionary<string, int>? visitedSmallCaveCount = null)
    {
        Debug.Assert(specialSmallCave != CaveEnd);
        Debug.Assert(specialSmallCave != CaveStart);

        currentRoute ??= new List<string>();
        visitedSmallCaveCount ??= new Dictionary<string, int>();

        currentRoute.Add(currentCaveName);
        if (currentCaveName.All(char.IsLower))
        {
            visitedSmallCaveCount.UpdateOrAdd(currentCaveName, i => i + 1, 1);
        }

        if (currentCaveName == endCave)
        {
            yield return string.Join(",", currentRoute);
            yield break;
        }

        var currentCave = allPaths[currentCaveName];
        foreach (var subCave in currentCave.Connections)
        {
            if (subCave.All(char.IsLower)
                && visitedSmallCaveCount.TryGetValue(subCave, out var visitedCount)
                && !AllowVisitCount(allPaths[subCave], specialSmallCave, visitedCount))
            {
                continue;
            }

            foreach (var subPath in GetFullFromCurrentToEndPaths2(allPaths, subCave, endCave, specialSmallCave, new List<string>(currentRoute), new Dictionary<string, int>(visitedSmallCaveCount)))
            {
                yield return subPath;
            }
        }
    }

    private static bool AllowVisitCount(Cave cave, string specialSmallCave, int visitedCount)
    {
        return cave.CaveType switch
        {
            CaveType.Big => true,
            CaveType.Small => visitedCount < (cave.Name == specialSmallCave ? 2 : 1),
            CaveType.StartEnd => visitedCount < 1,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}