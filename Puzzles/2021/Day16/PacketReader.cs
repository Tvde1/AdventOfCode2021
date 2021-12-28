using System;
using System.Collections.Generic;

namespace AdventOfCode.Puzzles._2021.Day16;

public class PacketReader
{
    private readonly TransmissionReader _transmissionReader;

    public PacketReader(TransmissionReader transmissionReader)
    {
        _transmissionReader = transmissionReader;
    }

    public IEnumerable<Packet> ReadAllPackets()
    {
        while (_transmissionReader.HasData)
        {
            yield return ParsePacket();
        }
    }

    private Packet ParsePacket()
    {
        var packetVersion = _transmissionReader.Read(3);
        var packetType = (PacketType) _transmissionReader.Read(3);

        return packetType switch
        {
            PacketType.Literal => ParseLiteralPacket(packetType, packetVersion),
            _ => ParseOperatorPacket(packetType, packetVersion),
        };
    }


    private LiteralPacket ParseLiteralPacket(PacketType packetType, int packetVersion)
    {
        var payload = 0L;

        while (true)
        {
            var part = _transmissionReader.Read(5);

            payload <<= 4;
            payload |= (byte) (part & 0b01111);

            if ((part & 0b10000) == 0)
            {
                break;
            }
        }

        return new LiteralPacket(packetVersion, packetType, payload);
    }


    private OperatorPacket ParseOperatorPacket(PacketType packetType, int packetVersion)
    {
        var read = _transmissionReader.Read(1);
        var lengthType = (LengthType)read;

        return lengthType switch
        {
            LengthType.SubPacketCount => ParseSubPacketCountOperatorPacket(packetType, packetVersion, lengthType),
            LengthType.TotalLength => ParseTotalLengthOperatorPacket(packetType, packetVersion, lengthType),
            _ => throw new ArgumentException(),
        };
    }

    private OperatorPacket ParseSubPacketCountOperatorPacket(PacketType packetType, int packetVersion,
        LengthType lengthType)
    {
        var subPacketCount = _transmissionReader.Read(11);

        var subPackets = new Packet[subPacketCount];
        for (var i = 0; i < subPacketCount; i++)
        {
            var parsedPacket = ParsePacket();

            subPackets[i] = parsedPacket ?? throw new ArgumentException();
        }

        return new OperatorPacket(packetVersion, packetType, lengthType, subPackets);
    }

    private OperatorPacket ParseTotalLengthOperatorPacket(PacketType packetType, int packetVersion,
        LengthType lengthType)
    {
        var subPacketLength = _transmissionReader.Read(15);

        var subPackets = new List<Packet>();

        var currentCount = _transmissionReader.Index;
        while (_transmissionReader.Index != currentCount + subPacketLength)
        {
            var parsedPacket = ParsePacket();
            if (parsedPacket is null)
            {
                throw new ArgumentException();
            }

            subPackets.Add(parsedPacket);
        }

        return new OperatorPacket(packetVersion, packetType, lengthType, subPackets.ToArray());
    }
}