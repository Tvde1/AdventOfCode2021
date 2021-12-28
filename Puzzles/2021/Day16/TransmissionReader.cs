using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace AdventOfCode.Puzzles._2021.Day16;

public class TransmissionReader
{
    private readonly BitArray _data;

    public TransmissionReader(string transmissionData)
    {
        _data = new BitArray(ConvertHexToBitArray(transmissionData));
    }

    public bool HasData => _data.Count - Index >= 8;
    public int Index { get; private set; }

    [DebuggerStepThrough]
    public int Read(int amount)
    {
        var b = 0;

        for (var i = 0; i < amount; i++)
        {
            b <<= 1;
            b |= _data.Get(Index++) ? 1 : 0;
        }

        return b;
    }

    private static BitArray ConvertHexToBitArray(string hexData)
    {
        var bitArray = new BitArray(4 * hexData.Length);
        for (var byteIndex = 0; byteIndex < hexData.Length; byteIndex++)
        {
            var b = byte.Parse(hexData[byteIndex].ToString(), NumberStyles.HexNumber);
            for (int bitIndex = 0; bitIndex < 4; bitIndex++)
            {
                bitArray.Set(byteIndex * 4 + bitIndex, (b & (1 << (3 - bitIndex))) != 0);
            }
        }
        return bitArray;
    }
}