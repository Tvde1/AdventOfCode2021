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
        : base(16, AdventDayImplementation.Build(AdventDataSource.FromFile(InputFile), HexToBits, PartOne))
    { }

    public static string PartOne(Queue<bool> data)
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

    private enum PacketType
    {
        Literal = 4,
        Operator,
    }

    private abstract class Packet
    {
        protected Packet(int version, PacketType type)
        {
            Version = version;
            Type = type;
        }

        public int Version { get; }
        public PacketType Type { get; }
    }

    private class LiteralPacket : Packet
    {
        public LiteralPacket(int version, PacketType type, int decimalValue)
               : base(version, type)
        {
            DecimalValue = decimalValue;
        }

        public int DecimalValue { get; }
    }

    private enum LengthType
    {
        TotalLength,
        SubPacketCount,
    }

    private class OperatorPacket : Packet
    {
        public OperatorPacket(int version, PacketType type, LengthType lengthType, Packet[] subPackets)
            : base(version, type)
        {
            LengthType = lengthType;
            SubPackets = subPackets;
        }

        public LengthType LengthType { get; }

        public Packet[] SubPackets { get; }
    }

    private static Packet[] ParsePackets(Queue<bool> bits)
    {
        var packets = new List<Packet>();

        while (true)
        {
            var packet = ParsePacket(bits);

            if (packet is null)
            {
                break;
            }

            packets.Add(packet);
        }

        return packets.ToArray();
    }

    private static Packet? ParsePacket(Queue<bool> bits)
    {
        if (!bits.TryDequeueAmount(3, out var packetVersionBits))
        {
            return null;
        }
        var packetVersion = packetVersionBits.ArrangeBits();

        if (!bits.TryDequeueAmount(3, out var packetTypeBits))
        {
            return null;
        }
        var packetType = (PacketType)packetTypeBits.ArrangeBits();

        switch (packetType)
        {
            case PacketType.Literal:
                {
                    var segments = new List<bool>();

                    while (true)
                    {
                        if (!bits.TryDequeueAmount(4, out var part))
                        {
                            throw new ArgumentException();
                        }

                        segments.AddRange(part);

                        if (!part[0])
                        {
                            break;
                        }
                    }

                    return new LiteralPacket(packetVersion, packetType, segments.ArrangeBits());
                }
            case PacketType.Operator:
                {
                    if (!bits.TryDequeue(out var lengthTypeBit))
                    {
                        throw new ArgumentException();
                    }
                    var lengthType = lengthTypeBit ? LengthType.TotalLength : LengthType.SubPacketCount;

                    switch (lengthType)
                    {
                        case LengthType.SubPacketCount:
                            {
                                if (!bits.TryDequeueAmount(11, out var subPacketCountBits))
                                {
                                    throw new ArgumentException();
                                }
                                var subPacketCount = subPacketCountBits.ArrangeBits();

                                var subPackets = new Packet[subPacketCount];
                                for(int i = 0; i < subPacketCount; i++)
                                {
                                    var parsedPacket = ParsePacket(bits);
                                    if (parsedPacket is null)
                                    {
                                        throw new ArgumentException();
                                    }
                                    subPackets[i] = parsedPacket;
                                }

                                return new OperatorPacket(packetVersion, packetType, lengthType, subPackets);
                            }
                        case LengthType.TotalLength:
                            {
                                if (!bits.TryDequeueAmount(15, out var subPacketLengthBits))
                                {
                                    throw new ArgumentException();
                                }
                                var subPacketLength = subPacketLengthBits.ArrangeBits();

                                var subPackets = new List<Packet>();

                                var currentCount = bits.Count;
                                while (bits.Count != currentCount - subPacketLength)
                                {
                                    var parsedPacket = ParsePacket(bits);
                                    if (parsedPacket is null)
                                    {
                                        throw new ArgumentException();
                                    }
                                    subPackets.Add(parsedPacket);
                                }

                                return new OperatorPacket(packetVersion, packetType, lengthType, subPackets.ToArray());
                            }
                        default:
                            {
                                throw new ArgumentOutOfRangeException();
                            }
                    }
                }
            default:
                throw new ArgumentOutOfRangeException();
        }

        throw new Oopsie();
    }

    private static Queue<bool> HexToBits(string hex)
    {
        var enumerable = hex.Select(x => x switch
        {
            '0' => (byte) 0b0000,
            '1' => (byte) 0b0001,
            '2' => (byte) 0b0010,
            '3' => (byte) 0b0011,
            '4' => (byte) 0b0100,
            '5' => (byte) 0b0101,
            '6' => (byte) 0b0110,
            '7' => (byte) 0b0111,
            '8' => (byte) 0b1000,
            '9' => (byte) 0b1001,
            'A' => (byte) 0b1010,
            'B' => (byte) 0b1011,
            'C' => (byte) 0b1100,
            'D' => (byte) 0b1101,
            'E' => (byte) 0b1110,
            'F' => (byte) 0b1111,
            _ => throw new ArgumentException(),
        }).SelectMany(x => x.ToBits(4));

        return new Queue<bool>(enumerable);
    }
}