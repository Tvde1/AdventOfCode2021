using System;
using System.Linq;

namespace AdventOfCode.Puzzles._2021.Day16;

public class TransmissionRunner
{
    public static long Run(Packet p)
    {
        return p switch
        {
            LiteralPacket literalPacket => literalPacket.DecimalValue,
            OperatorPacket operatorPacket => operatorPacket.Type switch
            {
                PacketType.Sum => operatorPacket.SubPackets.Sum(Run),
                PacketType.Product => operatorPacket.SubPackets.Select(Run).Aggregate(1L, (product, packetValue) => packetValue * product),
                PacketType.Minimum => operatorPacket.SubPackets.Min(Run),
                PacketType.Maximum => operatorPacket.SubPackets.Max(Run),
                PacketType.GreaterThan => Run(operatorPacket.SubPackets[0]) > Run(operatorPacket.SubPackets[1]) ? 1 : 0,
                PacketType.LessThan => Run(operatorPacket.SubPackets[0]) < Run(operatorPacket.SubPackets[1]) ? 1 : 0,
                PacketType.EqualTo => Run(operatorPacket.SubPackets[0]) == Run(operatorPacket.SubPackets[1]) ? 1 : 0,
                PacketType.Literal => throw new InvalidOperationException($"Operator packets cannot have a {nameof(OperatorPacket.Type)} of {nameof(PacketType.Literal)}"),
                _ => throw new ArgumentOutOfRangeException(nameof(operatorPacket.Type))
            },
            _ => throw new ArgumentOutOfRangeException(nameof(p))
        };
    }
}