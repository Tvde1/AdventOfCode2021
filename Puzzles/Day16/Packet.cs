using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Puzzles.Day16;

public enum PacketType : byte
{
    Literal = 4,
    Operator,
}

public abstract class Packet
{
    protected Packet(int version, PacketType type)
    {
        Version = version;
        Type = type;
    }

    public int Version { get; }
    public PacketType Type { get; }
}

public class LiteralPacket : Packet
{
    public LiteralPacket(int version, PacketType type, int decimalValue)
           : base(version, type)
    {
        DecimalValue = decimalValue;
    }

    public int DecimalValue { get; }
}

public enum LengthType
{
    TotalLength,
    SubPacketCount,
}

public class OperatorPacket : Packet
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

