using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Puzzles.Day18
{
    public abstract record class ReduceOperation
    {
        public static CompletedReduceOperation Done => new CompletedReduceOperation();

        public static ExplodeReduceOperation FromExplosion(PairSnailFishNumber explodedPair) => new(explodedPair);
        public static SplitReduceOperation FromSplit(SnailFishNumber replacePair) => new(replacePair);

    }

    public record CompletedReduceOperation : ReduceOperation
    {
    }

    public record class AddToLeftMostOperation(int Value) : ReduceOperation;
    public record class AddToRightMostOperation(int Value) : ReduceOperation;

    public record class ExplodeReduceOperation : ReduceOperation
    {
        public ExplodeReduceOperation(PairSnailFishNumber explodedPair)
        {
            AddLeft = new AddToRightMostOperation((explodedPair.Left as LiteralSnailFishNumber)!.Value);
            AddRight = new AddToLeftMostOperation((explodedPair.Right as LiteralSnailFishNumber)!.Value);
        }

        public bool RemoveMe { get; set; }

        public AddToRightMostOperation? AddLeft { get; set; }

        public AddToLeftMostOperation? AddRight { get; set; }

        public ReduceOperation WithLeftPerformed()
        {
            if (AddRight is null && !RemoveMe)
            {
                return ReduceOperation.Done;
            }

            return this with { AddLeft = null };
        }

        public ReduceOperation WithRightPerformed()
        {
            if (AddLeft is null && !RemoveMe)
            {
                return ReduceOperation.Done;
            }

            return this with { AddRight = null };
        }

        public ReduceOperation WithRemovedPerformed()
        {
            if (AddLeft is null && AddRight is null)
            {
                return ReduceOperation.Done;
            }

            return this with { RemoveMe = false };
        }
    }

    public record SplitReduceOperation : ReduceOperation
    {
        public SplitReduceOperation(SnailFishNumber replacePair)
        {
            ReplacePair = replacePair;
        }

        public SnailFishNumber ReplacePair { get; }
    }
}
