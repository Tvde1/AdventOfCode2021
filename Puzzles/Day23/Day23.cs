using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        : base(23, AdventDayImplementation.Build(TestInput, Parse, PartOne))
    { }

    private static Dictionary<int, AmphipodType> Parse(string input) => throw new NotImplementedException();

    private static string PartOne(Dictionary<int, AmphipodType> data)
    {
        var layout = AmphipodBurrowLayout.Default;

        var cost = AStar.Calculate(data,
            state => state.IsCompleted(layout),
            state => GetPossibleModifications(state, layout),
            (state, modification) => ApplyModification(state, modification),
            stateChange => GetCost(stateChange),
            stateChange => GetHeuristic(stateChange));

        return cost.ToString();

        throw new Oopsie();
    }

    private static string PartTwo(string data) => data;

    private static int GetCost(BurrowChange change)
    {
        return change.Current.Type switch
        {
            AmphipodType.A => 1,
            AmphipodType.B => 10,
            AmphipodType.C => 100,
            AmphipodType.D => 1000,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static IEnumerable<AmphipodBurrowModification> GetPossibleModifications(Dictionary<int, AmphipodType> state, AmphipodBurrowModification modification)
    {
        throw new NotImplementedException();
    }

    //private static int GetHeuristic(AmphipodBurrowState state)
    //{
    //    return 0;
    //}

    //private static AmphipodBurrowState ApplyModification(AmphipodBurrowState state, BurrowChange modification)
    //{
    //    return new AmphipodBurrowState(state.Amphipods.Select(x => x with { Location = x == modification.Previous ? m : x.Location }).ToList()),
    //}
}

public enum AmphipodType
{
    A,
    B,
    C,
    D,
}


public readonly record struct AmphipodBurrowModification(int OldLocation, int NewLocation)
{

}

public enum LocationState
{
    Hallway,
    RoomA,
    RoomB,
    RoomC,
    RoomD,
}

public static class AmphipodBurrowLayoutLogic
{
    public static Dictionary<int, LocationState> LocationStates = new Dictionary<int, LocationState>()
    {
        { 0, LocationState.Hallway },
        { 1, LocationState.Hallway },
        { 2, LocationState.Hallway },
        { 3, LocationState.Hallway },
        { 4, LocationState.Hallway },
        { 5, LocationState.Hallway },
        { 6, LocationState.Hallway },
        { 10, LocationState.RoomA },
        { 11, LocationState.RoomA },
        { 12, LocationState.RoomB },
        { 13, LocationState.RoomB },
        { 14, LocationState.RoomC },
        { 15, LocationState.RoomC },
        { 16, LocationState.RoomD },
        { 17, LocationState.RoomD },
    };

    public static IEnumerable<int> NextLocations(int location, ref List<int> locations)
    {
        int i = 0;
        switch(location)
        {
            case 0:
                {
                    locations.Add(1);
                    break;
                }
            case 1:
                {
                    locations.Add(0);
                    locations.Add(2);
                    break;
                }
            case 2:
                {
                    locations.Add(1);
                    locations.Add(3);
                    break;
                }
            case 3:
                {
                    locations.Add(2);
                    locations.Add(4);
                    break;
                }
            case 4:
                {
                    locations.Add(3);
                    locations.Add(5);
                    break;
                }
            case 5:
                {
                    locations.Add(4);
                    locations.Add(6);
                    break;
                }
            case 6:
                {
                    locations.Add(5);
                    break;
                }
        }

        return locations;
    }
}

//public record Amphipod(AmphipodType Type, AmphipodBurrowLocation Location)
//{
//    public int TimesMoved { get; set; }

//    public static Amphipod Parse(Group input, AmphipodBurrowLayout layout)
//    {
//        var startLocation = int.Parse(input.Name[1..]);

//        return new(Enum.Parse<AmphipodType>(input.Value), layout.Positions[startLocation]);
//    }
//}

//public enum BurrowLocationType
//{
//    Hallway,
//    Room,
//}

//public readonly record struct AmphipodBurrowLocation(int Location, BurrowLocationType Type, AmphipodType? RoomType)
//{
//    public static AmphipodBurrowLocation Room(int location, AmphipodType type) => new(location, BurrowLocationType.Room, type);
//    public static AmphipodBurrowLocation Hallway(int location) => new(location, BurrowLocationType.Hallway, null);
//}

//public readonly record struct AmphipodBurrowLayout(AmphipodBurrowLocation[] Positions,
//    Dictionary<AmphipodBurrowLocation, AmphipodBurrowLocation[]> Connections, Dictionary<AmphipodBurrowLocation, AmphipodType> Homes)
//{
//    static AmphipodBurrowLayout()
//    {
//        var positions = new[]
//        {
//            AmphipodBurrowLocation.Hallway(0), AmphipodBurrowLocation.Hallway(1), AmphipodBurrowLocation.Hallway(2), AmphipodBurrowLocation.Hallway(3), AmphipodBurrowLocation.Hallway(4), AmphipodBurrowLocation.Hallway(5), AmphipodBurrowLocation.Hallway(6),
//            AmphipodBurrowLocation.Room(7, AmphipodType.A), AmphipodBurrowLocation.Room(8, AmphipodType.B), AmphipodBurrowLocation.Room(9, AmphipodType.C), AmphipodBurrowLocation.Room(10, AmphipodType.D),
//            AmphipodBurrowLocation.Room(11, AmphipodType.A), AmphipodBurrowLocation.Room(12, AmphipodType.B), AmphipodBurrowLocation.Room(13, AmphipodType.C), AmphipodBurrowLocation.Room(14, AmphipodType.D)
//        }.ToDictionary(x => x.Location, x => x);

//        var connections = new Dictionary<int, int[]>
//            {
//                {0, new[] {1}},
//                {1, new[] {0, 2, 7}},
//                {2, new[] {1, 3, 7, 8}},
//                {3, new[] {2, 4, 8, 9}},
//                {4, new[] {3, 5, 9, 10}},
//                {5, new[] {4, 6, 10}},
//                {6, new[] {5, 7}},
//                {7, new[] {1, 2}},
//                {8, new[] {2, 3}},
//                {9, new[] {3, 4}},
//                {10, new[] {4, 5}},
//                {11, new[] {7}},
//                {12, new[] {8}},
//                {13, new[] {9}},
//                {14, new[] {10}}
//            }.ToDictionary(x => positions[x.Key], x => x.Value.Select(y => positions[y]).ToArray());

//        var homes = new Dictionary<int, AmphipodType>
//            {
//                {7, AmphipodType.A},
//                {11, AmphipodType.A},
//                {8, AmphipodType.B},
//                {12, AmphipodType.B},
//                {9, AmphipodType.C},
//                {13, AmphipodType.C},
//                {10, AmphipodType.D},
//                {14, AmphipodType.D}
//            }.ToDictionary(x => positions[x.Key], x => x.Value);

//        Default = new AmphipodBurrowLayout(positions.Values.ToArray(), connections, homes);
//    }

//    public static readonly AmphipodBurrowLayout Default;
//}

//public readonly record struct BurrowChange(Amphipod Previous, Amphipod Current);

//public readonly record struct AmphipodBurrowState(List<Amphipod> Amphipods)
//{
//    private static readonly Regex ParseRegex = new(@"##(?'P7'\w)#(?'P8'\w)#(?'P9'\w)#(?'P10'\w).+\n  #(?'P11'\w)#(?'P12'\w)#(?'P13'\w)#(?'P14'\w)");

//    public bool IsCompleted(AmphipodBurrowLayout layout)
//    {
//        return Amphipods.All(amphipod => amphipod.Location.RoomType == amphipod.Type);
//    }

//    public IEnumerable<(AmphipodBurrowState, BurrowChange)> GetNextStatesIEnumerable(AmphipodBurrowLayout layout)
//    {
//        foreach (var amphipod in Amphipods)
//        {
//            if (amphipod.TimesMoved == 2)
//            {
//                continue;
//            }

//            var possibleNextLocations = layout.Connections[amphipod.Location];

//            foreach (var possibleNextLocation in possibleNextLocations)
//            {
//                if (possibleNextLocation.Type == BurrowLocationType.Hallway && amphipod.TimesMoved == 1)
//                {
//                    continue;
//                }

//                if (Amphipods.Any(x => x.Location == possibleNextLocation))
//                {
//                    continue;
//                }

//                var newAmphipods = Amphipods.Select(x => x with { Location = x == amphipod ? possibleNextLocation : x.Location }).ToList();

//                yield return (new AmphipodBurrowState(newAmphipods), new BurrowChange(amphipod, amphipod with { Location = possibleNextLocation }));
//            }
//        }
//    }

//    public IEnumerable<BurrowChange> GetPossibleModifications(AmphipodBurrowLayout layout)
//    {
//        var amphipods = Amphipods;
//        return Amphipods.Where(x => x.TimesMoved != 2)
//            .SelectMany(amphipod => layout.Connections[amphipod.Location]
//                .Where(possibleNextLocation => (possibleNextLocation.Type != BurrowLocationType.Hallway || amphipod.TimesMoved != 1)
//                && (amphipods.All(x => x.Location != possibleNextLocation)))
//                .Select(possibleNextLocation => new BurrowChange(amphipod, amphipod with { Location = possibleNextLocation })));
//    }

//    public static AmphipodBurrowState Parse(string input, AmphipodBurrowLayout layout)
//    {
//        var match = ParseRegex.Match(input);

//        var amphipods = ((IReadOnlyDictionary<string, Group>)match.Groups).Where(group => group.Key != "0")
//            .Select(group => Amphipod.Parse(group.Value, layout)).ToList();

//        return new AmphipodBurrowState(amphipods);
//    }
//}

//public static class AmphipodLogic
//{

//}