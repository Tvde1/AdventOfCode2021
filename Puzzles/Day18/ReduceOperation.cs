using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Puzzles.Day18
{
    public enum ReduceStrategy
    {
        Explosions,
        Splits,
    }

    public abstract record class ReduceOperation
    {
        public static NoActionReduceOperation NoAction => new NoActionReduceOperation();
        protected static CompletedReduceOperation Completed(ReduceOperation operation) => new CompletedReduceOperation(operation);

        public static ExplodeReduceOperation FromExplosion(PairSnailFishNumber explodedPair) => new(explodedPair);
        public static SplitReduceOperation FromSplit(SnailFishNumber replacePair) => new(replacePair);

        public abstract override string ToString();
    }

    public record NoActionReduceOperation : ReduceOperation
    {
        public override string ToString()
        {
            return "noaction";
        }
    }

    public record CompletedReduceOperation(ReduceOperation Operation) : ReduceOperation
    {
        public override string ToString()
        {
            return $"{Operation}, completed";
        }
    }

    public record class AddToLeftMostOperation(int Value) : ReduceOperation
    {
        public override string ToString()
        {
            return "addleft";
        }
    }

    public record class AddToRightMostOperation(int Value) : ReduceOperation
    {
        public override string ToString()
        {
            return "addright";
        }
    }

    public record class ExplodeReduceOperation : ReduceOperation
    {
        public ExplodeReduceOperation(PairSnailFishNumber explodedPair)
        {
            RemoveMe = true;
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
                return Completed(this);
            }

            return this with { AddLeft = null };
        }

        public ReduceOperation WithRightPerformed()
        {
            if (AddLeft is null && !RemoveMe)
            {
                return Completed(this);
            }

            return this with { AddRight = null };
        }

        public ReduceOperation WithRemovedPerformed()
        {
            if (AddLeft is null && AddRight is null)
            {
                return Completed(this);
            }

            return this with { RemoveMe = false };
        }

        public override string ToString()
        {
            return "explode";
        }
    }

    public record SplitReduceOperation : ReduceOperation
    {
        public SplitReduceOperation(SnailFishNumber replacePair)
        {
            ReplacePair = replacePair;
        }

        public SnailFishNumber ReplacePair { get; }

        public ReduceOperation WithReplaced()
        {
            return Completed(this);
        }

        public override string ToString()
        {
            return "split";
        }
    }
}
