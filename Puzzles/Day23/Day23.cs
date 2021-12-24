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

    private static AmphipodType?[] Parse(string input) => AmphipodBurrowLayoutLogic.Parse(input);

    private static string PartOne(AmphipodType?[] data)
    {
        var cost = AStar.Calculate(data,
            AmphipodBurrowLayoutLogic.GetPossibleModifications,
            ApplyModification,
            GetCost,
            AmphipodBurrowLayoutLogic.GetHeuristic);

        return cost.ToString();
    }

    private static AmphipodType?[] ApplyModification(AmphipodType?[] state, (int OldLocation, int NewLocation, AmphipodType Moved) modification)
    {
        var newState = new AmphipodType?[state.Length];

        state.CopyTo(newState, 0);
        newState[modification.OldLocation] = null;
        newState[modification.NewLocation] = modification.Moved;

        return newState;
    }

    private static string PartTwo(string data) => data;

    private static int GetCost((int OldLocation, int NewLocation, AmphipodType Moved) modification)
    {
        return modification.Moved switch
        {
            AmphipodType.A => 1,
            AmphipodType.B => 10,
            AmphipodType.C => 100,
            AmphipodType.D => 1000,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
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
        { 12, LocationState.RoomA },
        { 13, LocationState.RoomA },
        { 14, LocationState.RoomB },
        { 15, LocationState.RoomB },
        { 16, LocationState.RoomB },
        { 17, LocationState.RoomB },
        { 18, LocationState.RoomC },
        { 19, LocationState.RoomC },
        { 20, LocationState.RoomC },
        { 21, LocationState.RoomC },
        { 22, LocationState.RoomD },
        { 23, LocationState.RoomD },
        { 24, LocationState.RoomD },
        { 25, LocationState.RoomD },
    };

    public static IEnumerable<(AmphipodType[] NewState, int Cost)> GetPossibleModifications(AmphipodType?[] locations)
    {
        var newLocations = new List<int>(10);
        for (var i = 0; i < locations.Length; i++)
        {
            var amphipod = locations[i];

            if (amphipod == null)
            {
                continue;
            }

            FillListWithNextLocations(i, ref newLocations);

            foreach (var newLocation in newLocations)
            {
                if (locations[newLocation] != null)
                {
                    continue;
                }

                if (IsInvalidMove(LocationStates[newLocation], amphipod.Value))
                {
                    continue;
                }

                var newState = new AmphipodType?[26];
                locations.CopyTo(newState);

                yield return (i, newLocation, amphipod.Value);
            }
        }
    }

    private static bool IsInvalidMove(LocationState locationState, AmphipodType amphipod)
    {
        return (locationState, amphipod) switch
        {
            (LocationState.RoomA, AmphipodType.A) => false,
            (LocationState.RoomB, AmphipodType.B) => false,
            (LocationState.RoomC, AmphipodType.C) => false,
            (LocationState.RoomD, AmphipodType.D) => false,
            (LocationState.Hallway, _) => false,
            _ => true,
        };
    }

    private static bool IsCorrectHome(LocationState locationState, AmphipodType amphipod)
    {
        return (locationState, amphipod) switch
        {
            (LocationState.RoomA, AmphipodType.A) => true,
            (LocationState.RoomB, AmphipodType.B) => true,
            (LocationState.RoomC, AmphipodType.C) => true,
            (LocationState.RoomD, AmphipodType.D) => true,
            (LocationState.Hallway, _) => false,
            _ => false,
        };
    }

    public static void FillListWithNextLocations(int location, ref List<int> locations)
    {
        locations.Clear();
        switch (location)
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
                    locations.Add(10);
                    locations.Add(11);
                    locations.Add(12);
                    locations.Add(13);
                    break;
                }
            case 2:
                {
                    locations.Add(1);
                    locations.Add(3);
                    locations.Add(10);
                    locations.Add(11);
                    locations.Add(12);
                    locations.Add(13);
                    locations.Add(14);
                    locations.Add(15);
                    locations.Add(16);
                    locations.Add(17);
                    break;
                }
            case 3:
                {
                    locations.Add(2);
                    locations.Add(4);
                    locations.Add(14);
                    locations.Add(15);
                    locations.Add(16);
                    locations.Add(17);
                    locations.Add(18);
                    locations.Add(19);
                    locations.Add(20);
                    locations.Add(21);
                    break;
                }
            case 4:
                {
                    locations.Add(3);
                    locations.Add(5);
                    locations.Add(18);
                    locations.Add(19);
                    locations.Add(20);
                    locations.Add(21);
                    locations.Add(22);
                    locations.Add(23);
                    locations.Add(24);
                    locations.Add(25);
                    break;
                }
            case 5:
                {
                    locations.Add(4);
                    locations.Add(6);
                    locations.Add(22);
                    locations.Add(23);
                    locations.Add(24);
                    locations.Add(25);
                    break;
                }
            case 6:
                {
                    locations.Add(5);
                    break;
                }
            case 10:
            case 11:
            case 12:
            case 13:
                {
                    locations.Add(1);
                    locations.Add(2);
                    break;
                }
            case 14:
            case 15:
            case 16:
            case 17:
            {
                locations.Add(2);
                locations.Add(3);
                break;
            }
            case 18:
            case 19:
            case 20:
            case 21:
            {
                locations.Add(3);
                locations.Add(4);
                break;
            }
            case 22:
            case 23:
            case 24:
            case 25:
            {
                locations.Add(4);
                locations.Add(5);
                break;
            }
        }
    }

    public static int GetHeuristic(AmphipodType?[] state)
    {
        var notHomeCount = state.Where((amphipodType, i) =>
                amphipodType != null && !IsCorrectHome(LocationStates[i], amphipodType.Value))
            .Count();

        return notHomeCount;
    }

    private static readonly Regex ParseRegex = new(@"##(?'A1'\w)#(?'B1'\w)#(?'C1'\w)#(?'D1'\w).+\n  #(?'A2'\w)#(?'B2'\w)#(?'C2'\w)#(?'D2'\w)");

    public static AmphipodType?[] Parse(string input)
    {
        var arr = new AmphipodType?[26];

        void Add(string type, int index)
        {
            arr[index] = Enum.Parse<AmphipodType>(type);
        }

        var match = ParseRegex.Match(input);
        {
            Add(match.Groups["A1"].Value, 10);
            Add(match.Groups["A2"].Value, 11);
            Add(match.Groups["B1"].Value, 14);
            Add(match.Groups["B2"].Value, 15);
            Add(match.Groups["C1"].Value, 18);
            Add(match.Groups["C2"].Value, 19);
            Add(match.Groups["D1"].Value, 22);
            Add(match.Groups["D2"].Value, 23);
        }

        return arr;
    }

    public static AmphipodType?[] Unfold(AmphipodType?[] input)
    {
        var toInsert = @"#D#C#B#A#
#D#B#A#C#";
        return null;
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