using System;

namespace AdventOfCode.Puzzles.Day15;

public partial class Day15
{
    private class Oopsie : Exception
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
}