using System;

namespace AdventOfCode.Common;

public class Oopsie : Exception
{
    public Oopsie()
        : base("This should not be possible")
    {
    }

    public Oopsie(string message)
        : base(message)
    {
    }
}