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
            (state, mod) => mod.NewState,
            mod => mod.Cost,
            AmphipodBurrowLayoutLogic.GetHeuristic);

        return cost.ToString();
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


public readonly record struct AmphipodBurrowModification(int OldLocation, int NewLocation)
{

}

public enum LocationType
{
    Hallway,
    RoomA,
    RoomB,
    RoomC,
    RoomD,
}

public static class AmphipodBurrowLayoutLogic
{
    public static int[] HallwayLocations = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    public static Dictionary<int, int> RoomToHallwayConnections = new Dictionary<int, int>
    {
        { 11, 3 },
        { 15, 5 },
        { 19, 7 },
        { 23, 9},
    };

    public static Dictionary<AmphipodType, int> AmphipodTypeToHallwayConnections = new Dictionary<AmphipodType, int>()
    {
        { AmphipodType.A, 3 },
        { AmphipodType.B, 5 },
        { AmphipodType.C, 7 },
        { AmphipodType.D, 9 },
    };

    public static Dictionary<int, LocationType> RoomLocations = new Dictionary<int, LocationType>
    {
        { 11, LocationType.RoomA },
        { 12, LocationType.RoomA },
        { 13, LocationType.RoomA },
        { 14, LocationType.RoomA },
        { 15, LocationType.RoomB },
        { 16, LocationType.RoomB },
        { 17, LocationType.RoomB },
        { 18, LocationType.RoomB },
        { 19, LocationType.RoomC },
        { 20, LocationType.RoomC },
        { 21, LocationType.RoomC },
        { 22, LocationType.RoomC },
        { 23, LocationType.RoomD },
        { 24, LocationType.RoomD },
        { 25, LocationType.RoomD },
        { 26, LocationType.RoomD },
    };

    public static Dictionary<AmphipodType, int[]> RoomsFor = RoomLocations.Select(x => (Num: x.Key, Type: x.Value switch
    {
        LocationType.RoomA => AmphipodType.A,
        LocationType.RoomB => AmphipodType.B,
        LocationType.RoomC => AmphipodType.C,
        LocationType.RoomD => AmphipodType.D,
        _ => throw new ArgumentException(),
    })).GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.Select(y => y.Num).ToArray());

    public static Dictionary<int, LocationType> LocationStates = HallwayLocations.ToDictionary(x => x, x => LocationType.Hallway)
        .Union(RoomLocations).ToDictionary(x => x.Key, x => x.Value);

    private static IEnumerable<(int Location, int Cost)> GetMovesIntoRoom(AmphipodType?[] state, AmphipodType amphipod)
    {
        var roomType = amphipod switch
        {
            AmphipodType.A => LocationType.RoomA,
            AmphipodType.B => LocationType.RoomB,
            AmphipodType.C => LocationType.RoomC,
            AmphipodType.D => LocationType.RoomD,
            _ => throw new ArgumentException(),
        };

        var roomsForType = RoomLocations.Where(x => x.Value == roomType);

        var i = 0;
        foreach (var room in roomsForType)
        {
            i++;
            if (state[room.Key] == null)
            {
                yield return (room.Key, i);
            }
        }
    }

    private static IEnumerable<(int Location, int Cost)> GetHallwayMoves(AmphipodType?[] state, int location)
    {
        var cost = 1;
        for (var toTheLeft = location - 1; toTheLeft > 0; toTheLeft--)
        {
            if (state[toTheLeft] != null)
            {
                break;
            }

            yield return (toTheLeft, cost);
            cost++;
        }

        cost = 1;
        for (var toTheRight = location + 1; toTheRight < 11; toTheRight++)
        {
            if (state[toTheRight] != null)
            {
                break;
            }

            yield return (toTheRight, cost);
            cost++;
        }
    }

    private static IEnumerable<(int Location, int Cost)> GetMoveOutOfRoom(AmphipodType?[] state, int location)
    {
        var currentLocation = location;
        int cost = 1;
        while (true)
        {
            if (RoomToHallwayConnections.TryGetValue(currentLocation, out var hallwayConnection))
            {
                var furtherMoves = GetHallwayMoves(state, hallwayConnection);

                foreach (var hallwayMove in furtherMoves)
                {
                    yield return (hallwayMove.Location, hallwayMove.Cost + cost);
                }

                yield break;
            }

            currentLocation--;
            cost++;

            if (state[currentLocation] != null)
            {
                yield break;
            }
        }
    }

    public static IEnumerable<(AmphipodType?[] NewState, int Cost)> GetPossibleModifications(AmphipodType?[] state)
    {
        for (var i = 0; i < state.Length; i++)
        {
            if (!state[i].HasValue)
            {
                continue;
            }

            var amphipod = state[i]!.Value;

            var roomType = LocationStates[i];

            if (IsCorrectHome(roomType, amphipod))
            {
                // We do not know if another one is stuck a room below us
                //continue;
            }

            var costMultiplier = amphipod switch
            {
                AmphipodType.A => 1,
                AmphipodType.B => 10,
                AmphipodType.C => 100,
                AmphipodType.D => 1000,
                _ => throw new ArgumentOutOfRangeException(),
            };

            IEnumerable<(int Location, int Cost)> possibleMoves;

            switch (roomType)
            {
                case LocationType.Hallway:
                    {
                        var connection = AmphipodTypeToHallwayConnections[amphipod];

                        var min = Math.Min(i, connection);
                        var max = Math.Min(connection, i);

                        for (var moveInbetween = min; min < max; moveInbetween++)
                        {
                            if (state[moveInbetween] != null)
                            {
                                continue;
                            }
                        }

                        var costToMoveToConnection = max - min;

                        possibleMoves = GetMovesIntoRoom(state, amphipod).Select(x => (x.Location, x.Cost + costToMoveToConnection));
                        break;
                    }
                default:
                    {
                        possibleMoves = GetMoveOutOfRoom(state, i);
                        break;
                    }
            }

            foreach (var newLocation in possibleMoves)
            {
                var newState = new AmphipodType?[27];
                state.CopyTo(newState, 0);

                newState[i] = null;
                newState[newLocation.Location] = amphipod;

                yield return (newState, newLocation.Cost);
            }
        }
    }

    private static bool IsInvalidMove(LocationType locationState, AmphipodType amphipod)
    {
        return (locationState, amphipod) switch
        {
            (LocationType.RoomA, AmphipodType.A) => false,
            (LocationType.RoomB, AmphipodType.B) => false,
            (LocationType.RoomC, AmphipodType.C) => false,
            (LocationType.RoomD, AmphipodType.D) => false,
            (LocationType.Hallway, _) => false,
            _ => true,
        };
    }

    private static bool IsCorrectHome(LocationType locationState, AmphipodType amphipod)
    {
        return (locationState, amphipod) switch
        {
            (LocationType.RoomA, AmphipodType.A) => true,
            (LocationType.RoomB, AmphipodType.B) => true,
            (LocationType.RoomC, AmphipodType.C) => true,
            (LocationType.RoomD, AmphipodType.D) => true,
            (LocationType.Hallway, _) => false,
            _ => false,
        };
    }

    private static void GetPossibleMoves(int location, ref List<int> locations)
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
        var arr = new AmphipodType?[27];

        void Add(string type, int index)
        {
            arr[index] = Enum.Parse<AmphipodType>(type);
        }

        var match = ParseRegex.Match(input);
        {
            Add(match.Groups["A1"].Value, 11);
            Add(match.Groups["A2"].Value, 12);
            Add(match.Groups["B1"].Value, 15);
            Add(match.Groups["B2"].Value, 16);
            Add(match.Groups["C1"].Value, 18);
            Add(match.Groups["C2"].Value, 20);
            Add(match.Groups["D1"].Value, 23);
            Add(match.Groups["D2"].Value, 24);
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