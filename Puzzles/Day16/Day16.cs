using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day16;

public class Day16 : AdventDay
{
    private static readonly AdventDataSource TestInput1 = AdventDataSource.FromRaw("D2FE28");
    private static readonly AdventDataSource TestInput2 = AdventDataSource.FromRaw("38006F45291200");
    private static readonly AdventDataSource TestInput3 = AdventDataSource.FromRaw("EE00D40C823060");

    private static readonly AdventDataSource TestInput4 = AdventDataSource.FromRaw("C200B40A82");
    private static readonly AdventDataSource TestInput5 = AdventDataSource.FromRaw("04005AC33890");
    private static readonly AdventDataSource TestInput6 = AdventDataSource.FromRaw("880086C3E88112");
    private static readonly AdventDataSource TestInput7 = AdventDataSource.FromRaw("CE00C43D881120");
    private static readonly AdventDataSource TestInput8 = AdventDataSource.FromRaw("D8005AC2A8F0");
    private static readonly AdventDataSource TestInput9 = AdventDataSource.FromRaw("F600BC2D8F");
    private static readonly AdventDataSource TestInputA = AdventDataSource.FromRaw("9C005AC2F8F0");
    private static readonly AdventDataSource TestInputB = AdventDataSource.FromRaw("9C0141080250320F1802104A08");

    private static readonly AdventDataSource TestInputX = AdventDataSource.FromRaw("D2FE28");

    private static readonly AdventDataSource RealInput = AdventDataSource.FromFile("Day16/day16.txt");

    public Day16()
        : base(16, AdventDayImplementation.Build(RealInput, Lambda.Identity, PartOne, PartTwo))
    { }

    public static string PartOne(string data)
    {
        var transmission = Transmission.Parse(data);

        var flattened = new List<Packet>();
        void FlattenPacket(Packet p)
        {
            switch (p)
            {
                case LiteralPacket:
                {
                    flattened.Add(p);
                    break;
                }
                case OperatorPacket o:
                {
                    flattened.Add(p);
                    foreach (var sp in o.SubPackets)
                    {
                        FlattenPacket(sp);
                    }

                    break;
                }
                default:
                {
                    throw new ArgumentException();
                }
            }
        }

        foreach (var p in transmission.Packets)
        {
            FlattenPacket(p);
        }

        return flattened.Aggregate(0, (sum, packet) => sum + packet.Version).ToString();
    }

    public static string PartTwo(string data)
    {
        var transmission = Transmission.Parse(data);

        var result = transmission.Run();

        return result.ToString();
    }
}
