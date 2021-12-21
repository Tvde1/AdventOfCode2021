using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace AdventOfCode.Puzzles.Day16;

public record Transmission
{
    public Transmission(string data)
    {
        var packetReader = new PacketReader(new TransmissionReader(data));

        Packets = packetReader.ReadAllPackets().ToArray();
    }

    public IReadOnlyList<Packet> Packets { get; set; }
    
    public static Transmission Parse(string input) => new(input);

    public long Run()
    {
        return TransmissionRunner.Run(Packets[0]);
    }
}

public class TransmissionRunner
{
    public static long Run(Packet p)
    {
        return p switch
        {
            LiteralPacket literalPacket => Run(literalPacket),
            OperatorPacket operatorPacket => Run(operatorPacket),
            _ => throw new ArgumentOutOfRangeException(nameof(p))
        };
    }

    public static long Run(LiteralPacket literalPacket)
    {
        return literalPacket.DecimalValue;
    }

    public static long Run(OperatorPacket operatorPacket)
    {
        return operatorPacket.Type switch
        {
            PacketType.Sum => operatorPacket.SubPackets.Sum(Run),
            PacketType.Product => operatorPacket.SubPackets.Select(Run).Aggregate(1L, (product, packetValue) => packetValue * product),
            PacketType.Minimum => operatorPacket.SubPackets.Min(Run),
            PacketType.Maximum => operatorPacket.SubPackets.Max(Run),
            PacketType.GreaterThan => Run(operatorPacket.SubPackets[0]) > Run(operatorPacket.SubPackets[1]) ? 1 : 0,
            PacketType.LessThan => Run(operatorPacket.SubPackets[0]) < Run(operatorPacket.SubPackets[1]) ? 1 : 0,
            PacketType.EqualTo => Run(operatorPacket.SubPackets[0]) == Run(operatorPacket.SubPackets[1]) ? 1 : 0,
            PacketType.Literal => throw new InvalidOperationException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}