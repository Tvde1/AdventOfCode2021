using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles.Day23;

public class Day23 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.FromFile("Day23/day23.txt");

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"#############
#...........#
###B#C#B#D###
  #A#D#C#A#
  #########");

    public Day23()
        : base(23, AdventDayImplementation.Build(TestInput, AmphipodBurrowState.Parse))
    { }

    private static string PartOne(AmphipodBurrowState data)
    {
        var layout = AmphipodBurrowLayout.Default;

        var cost = AStar.Calculate(data,
            state => state.IsCompleted(layout),
            state => state.GetNextStates(layout),

            );



        throw new Oopsie();
    }

    private static string PartTwo(string data) => data;
}

public enum AmphipodType
{
    A,
    B,
    C,
    D,
}

public readonly record struct Amphipod(int StartLocation, AmphipodType Type)
{
    public static Amphipod Parse(Group input) => new(int.Parse(input.Name[1..]), Enum.Parse<AmphipodType>(input.Value));
}


public readonly record struct AmphipodBurrowLayout(int[] Positions,
    Dictionary<int, int[]> Connections, Dictionary<int, AmphipodType> Homes)
{
    public static AmphipodBurrowLayout Default
    {
        get
        {
            var positions = new[]
            {
                0, 1, 2, 3, 4, 5, 6,
                7, 8, 9, 10,
                11, 12, 13, 14
            };

            var connections = new Dictionary<int, int[]>
            {
                {0, new[] {1}},
                {1, new[] {0, 2, 7}},
                {2, new[] {1, 3, 7, 8}},
                {3, new[] {2, 4, 8, 9}},
                {4, new[] {3, 5, 9, 10}},
                {5, new[] {4, 6, 10}},
                {6, new[] {5, 7}},
                {7, new[] {1, 2}},
                {8, new[] {2, 3}},
                {9, new[] {3, 4}},
                {10, new[] {4, 5}},
                {11, new[] {7}},
                {12, new[] {8}},
                {13, new[] {9}},
                {14, new[] {10}}
            };

            var homes = new Dictionary<int, AmphipodType>
            {
                {7, AmphipodType.A},
                {11, AmphipodType.A},
                {8, AmphipodType.B},
                {12, AmphipodType.B},
                {9, AmphipodType.C},
                {13, AmphipodType.C},
                {10, AmphipodType.D},
                {14, AmphipodType.D}
            };

            return new AmphipodBurrowLayout(positions, connections, homes);
        }
    }
}

public readonly record struct AmphipodBurrowState(Dictionary<Amphipod, int> Locations)
{
    private static readonly Regex ParseRegex = new(@"##(?'P7'\w)#(?'P8'\w)#(?'P9'\w)#(?'P10'\w).+\n  #(?'P11'\w)#(?'P12'\w)#(?'P13'\w)#(?'P14'\w)");

    public bool IsCompleted(AmphipodBurrowLayout layout)
    {
        return Locations.All(loc => layout.Homes.TryGetValue(loc.Value, out var type) && type == loc.Key.Type);
    }

    public IEnumerable<AmphipodBurrowState> GetNextStatesIEnumerable(AmphipodBurrowLayout layout)
    {
        foreach (var (amphipod, currentLocation) in Locations)
        {
            var possibleNextLocations = layout.Connections[currentLocation];

            foreach (var possibleNextLocation in possibleNextLocations)
            {
                if (Locations.ContainsValue(possibleNextLocation))
                {
                    continue;
                }

                var newLocations = Locations.ToDictionary(x => x.Key, x => x.Value);
                newLocations[amphipod] = possibleNextLocation;

                yield return new AmphipodBurrowState(newLocations);
            }
        }
    }


    public IEnumerable<AmphipodBurrowState> GetNextStates(AmphipodBurrowLayout layout)
    {
        var locations = Locations;
        
        return locations.SelectMany(currentLocation => layout.Connections[currentLocation.Value]
            .Where(possibleNextLocation => !locations.ContainsValue(possibleNextLocation)).Select(
                possibleNextLocation =>
                {
                    var newLocations = locations.ToDictionary(x => x.Key, x => x.Value);
                    newLocations[currentLocation.Key] = possibleNextLocation;

                    return new AmphipodBurrowState(newLocations);
                }));
    }

    public static AmphipodBurrowState Parse(string input)
    {
        var match = ParseRegex.Match(input);

        var locations = ((IReadOnlyDictionary<string, Group>) match.Groups).Where(group => group.Key != "0")
            .Select(group => Amphipod.Parse(group.Value))
            .ToDictionary(x => x, x => x.StartLocation);

        return new AmphipodBurrowState(locations);
    }
}

public static class AmphipodLogic
{

}