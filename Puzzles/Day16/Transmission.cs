using System.Collections;
using System.Globalization;

namespace AdventOfCode.Puzzles.Day16;

public record Transmission
{
    private BitArray _data;
    private int _index = 0;

    private Transmission(string input)
    {
        _data = new BitArray(ConvertHexToBitArray(input));
    }

    public bool Read()
    {
        return _data.Get(_index++);
    }

    public byte ReadTree()
    {
        byte b = 0;

        var b1 = Read();
        var b2 = Read();
        var b3 = Read();

        b |= b1 ? (byte)1 : (byte)0;
        b <<= 1;
        b |= b2 ? (byte)1 : (byte)0;
        b <<= 1;
        b |= b3 ? (byte)1 : (byte)0;

        _index += 3;

        return b;
    }

    public byte ReadFour()
    {
        byte b = 0;

        var b1 = Read();
        var b2 = Read();
        var b3 = Read();
        var b4 = Read();

        b |= b1 ? (byte)1 : (byte)0;
        b <<= 1;
        b |= b2 ? (byte)1 : (byte)0;
        b <<= 1;
        b |= b3 ? (byte)1 : (byte)0;
        b <<= 1;
        b |= b4 ? (byte)1 : (byte)0;

        _index += 4;

        return b;
    }

    public static Transmission Parse(string input) => new Transmission(input);

    private static BitArray ConvertHexToBitArray(string hexData)
    {
        var bitArray = new BitArray(4 * hexData.Length);
        for (int byteIndex = 0; byteIndex < hexData.Length; byteIndex++)
        {
            byte b = byte.Parse(hexData[byteIndex].ToString(), NumberStyles.HexNumber);
            for (int bitIndex = 0; bitIndex < 4; bitIndex++)
            {
                bitArray.Set(byteIndex * 4 + bitIndex, (b & (1 << (3 - bitIndex))) != 0);
            }
        }
        return bitArray;
    }
}