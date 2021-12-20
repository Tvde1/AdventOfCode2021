using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day16;

public class Day16 : AdventDay
{
    private const string InputFile = "Day16/day16.txt";

    private const string TestInput = @"8A004A801A8002F478";

    public Day16()
        : base(16, AdventDayImplementation.Build(AdventDataSource.FromFile(InputFile), Transmission.Parse, PartOne))
    { }

    public static string PartOne(Transmission data)
    {
        var packets = new Queue<Packet>(ParsePackets(data));

        var flattened = new List<Packet>();
        while (packets.TryDequeue(out var p))
        {
            switch (p)
            {
                case LiteralPacket l:
                    {
                        flattened.Add(p);
                        break;
                    }
                case OperatorPacket o:
                    {
                        flattened.Add(p);
                        foreach (var sp in o.SubPackets) {
                            packets.Enqueue(sp);
                        }
                        break;
                    }
                default:
                    {
                        throw new ArgumentException();
                    }
            }
        }

        return flattened.Aggregate(0, (sum, packet) => sum + packet.Version).ToString();
    }

    private static Packet[] ParsePackets(Transmission data)
    {
        var packets = new List<Packet>();

        while (true)
        {
            var packet = ParsePacket(data);

            if (packet is null)
            {
                break;
            }

            packets.Add(packet);
        }

        return packets.ToArray();
    }

    private static Packet? ParsePacket(Transmission transmission)
    {

        //var packetVersion = transmission.ReadTree();
        //var packetType = (PacketType) transmission.ReadTree();

        //switch (packetType)
        //{
        //    case PacketType.Literal:
        //        {
        //            var segments = new List<byte>();

        //            while (true)
        //            {
        //                var part = transmission.ReadFour();

        //                segments.AddRange(part);

        //                if ((part & 0b1000) == 0)
        //                {
        //                    break;
        //                }
        //            }

        //            var realValue = BitConverter.ToInt32(segments.ToArray());

        //            return new LiteralPacket(packetVersion, packetType, realValue);
        //        }
        //    case PacketType.Operator:
        //        {
        //            if (!transmission.TryDequeue(out var lengthTypeBit))
        //            {
        //                throw new ArgumentException();
        //            }
        //            var lengthType = lengthTypeBit ? LengthType.TotalLength : LengthType.SubPacketCount;

        //            switch (lengthType)
        //            {
        //                case LengthType.SubPacketCount:
        //                    {
        //                        if (!transmission.TryDequeueAmount(11, out var subPacketCountBits))
        //                        {
        //                            throw new ArgumentException();
        //                        }
        //                        var subPacketCount = subPacketCountBits.ArrangeBits();

        //                        var subPackets = new Packet[subPacketCount];
        //                        for(int i = 0; i < subPacketCount; i++)
        //                        {
        //                            var parsedPacket = ParsePacket(transmission);
        //                            if (parsedPacket is null)
        //                            {
        //                                throw new ArgumentException();
        //                            }
        //                            subPackets[i] = parsedPacket;
        //                        }

        //                        return new OperatorPacket(packetVersion, packetType, lengthType, subPackets);
        //                    }
        //                case LengthType.TotalLength:
        //                    {
        //                        if (!transmission.TryDequeueAmount(15, out var subPacketLengthBits))
        //                        {
        //                            throw new ArgumentException();
        //                        }
        //                        var subPacketLength = subPacketLengthBits.ArrangeBits();

        //                        var subPackets = new List<Packet>();

        //                        var currentCount = transmission.Count;
        //                        while (transmission.Count != currentCount - subPacketLength)
        //                        {
        //                            var parsedPacket = ParsePacket(transmission);
        //                            if (parsedPacket is null)
        //                            {
        //                                throw new ArgumentException();
        //                            }
        //                            subPackets.Add(parsedPacket);
        //                        }

        //                        return new OperatorPacket(packetVersion, packetType, lengthType, subPackets.ToArray());
        //                    }
        //                default:
        //                    {
        //                        throw new ArgumentOutOfRangeException();
        //                    }
        //            }
        //        }
        //    default:
        //        throw new ArgumentOutOfRangeException();
        //}

        throw new Oopsie();
    }
}
