using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Puzzles._2021.Day16;

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