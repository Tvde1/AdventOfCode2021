using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day16;

public class Day16 : AdventDayBase
{
    private const string InputFile = "Day16/day16.txt";

    private const string TestInput = @"";

    public Day16()
        : base(16)
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

    private enum PacketType
    {
        Literal = 4,
        Operator,
    }

    private abstract class Packet
    {
        public int Version { get; init; }
        public PacketType Type { get; init; }
    }

    private class LiteralPacket : Packet
    {
        public int DecimalValue { get; init; }
    }

    private enum LengthType
    {
        TotalLength,
        SubPacketCount,
    }

    private class OperatorPacket : Packet
    {
        public LengthType LengthType { get; }
    }

    private static Packet[] ParsePacket(string input)
    {
        var packets = new List<Packet>();

        Queue<bool> bits = HexToBits(input);

        while (bits.Any())
        {
            if (!bits.TryDequeueAmount(3, out var packetVersionBits))
            {
                break;
            }
            var packetVersion = packetVersionBits.ArrangeBits();

            if (!bits.TryDequeueAmount(3, out var packetTypeBits))
            {
                break;
            }
            var packetType = (PacketType) packetTypeBits.ArrangeBits();

            switch (packetType)
            {
                case PacketType.Literal:
                {
                    var segments = new List<bool[]>();

                    while (true)
                    {
                        if (!bits.TryDequeueAmount(4, out var part))
                        {
                            throw new ArgumentException();
                        }

                        segments.Add(part);

                        if (!part[0])
                        {
                            break;
                        }
                    }

                    packets.Add(new LiteralPacket
                    {

                    });

                    break;
                }
                case PacketType.Operator:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return packets.ToArray();
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
        }).EnumerateToBits(4);

        return new Queue<bool>(enumerable);
    }
}