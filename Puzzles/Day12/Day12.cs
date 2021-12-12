using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Monads;

namespace AdventOfCode.Puzzles.Day12;

public class Day12 : AdventDayBase
{
    private const string InputFile = "Day12/day12.txt";

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
        : base(12)
    {
        AddPart(PartOne);
        AddPart(PartTwo);
    }

    public static AdventAssignment PartOne =>
        AdventAssignment.Build(
            InputFile,
            input => GetAllCaveConnections(input.Split(Environment.NewLine)),
            data =>
            {
                return GetFullFromCurrentToEndPaths(data, CaveStart, CaveEnd).Count();
                //return string.Join(Environment.NewLine, GetFullFromCurrentToEndPaths(data, CaveStart, CaveEnd));
            });

    public static AdventAssignment PartTwo =>
        AdventAssignment.Build(
            InputFile,
            input => GetAllCavesFull(input.Split(Environment.NewLine)),
            data =>
            {
                var aaa = data.Values
                    .Where(cave => cave.CaveType == CaveType.Small).ToList();

                var bb=aaa
                    .SelectMany(c => GetFullFromCurrentToEndPaths2(data, CaveStart, CaveEnd, c.Name))
                    .Distinct();

                // return bb.Count();

                return
                //string.Join(Environment.NewLine, bb) + Environment.NewLine + Environment.NewLine + 
                bb.Count();
            });

    private static ILookup<string, string> GetAllCaveConnections(IEnumerable<string> inputs)
    {
        var split = inputs.Select(x => x.Split('-'));

        return split
            .Union(split.Select(x => x.Reverse().ToArray()))
            .ToLookup(x => x[0], x => x[1]);
    }

    private static IEnumerable<string> GetFullFromCurrentToEndPaths(ILookup<string, string> allPaths, string currentCave, string endCave,
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

        var currentPaths = allPaths[currentCave];
        foreach (var subCave in currentPaths)
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
                if (cave.Key == CaveEnd || cave.Key == CaveStart)
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
                Connections = cave.OrderBy(x=>x).ToArray(),
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
        switch (cave.CaveType)
        {
            case CaveType.Big:
                return true;
            case CaveType.Small:
                return visitedCount < (cave.Name == specialSmallCave ? 2 : 1);
            case CaveType.StartEnd:
                return visitedCount < 1;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}