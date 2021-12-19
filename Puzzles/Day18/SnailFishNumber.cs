using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AdventOfCode.Puzzles.Day18
{
    public abstract class SnailFishNumber
    {
        public abstract ReduceOperation Reduce(int depth = 0);

        public abstract bool PerformReduceOperation(ReduceOperation operation);

        public abstract int CalculateMagnitude();

        public static SnailFishNumber Add(SnailFishNumber left, SnailFishNumber right)
        {
            return new PairSnailFishNumber(left, right);
        }

        public static SnailFishNumber Parse(string input)
        {
            return ParseInternal(new Queue<char>(input));
        }

        public static SnailFishNumber ParseInternal(Queue<char> input)
        {
            var firstChar = input.Dequeue();

            if (firstChar == '[')
            {
                var firstNumber = SnailFishNumber.ParseInternal(input);

                var dequeueComma = input.Dequeue();
                Debug.Assert(dequeueComma == ',');

                var secondNumber = SnailFishNumber.ParseInternal(input);

                var dequeueClosing = input.Dequeue();
                Debug.Assert(dequeueClosing == ']');

                return new PairSnailFishNumber(firstNumber, secondNumber);
            }
            else
            {
                return new LiteralSnailFishNumber((int)char.GetNumericValue(firstChar));
            }
        }
    }

    public class PairSnailFishNumber : SnailFishNumber
    {
        public PairSnailFishNumber(SnailFishNumber left, SnailFishNumber right)
        {
            Left = left;
            Right = right;
        }

        public SnailFishNumber Left { get; private set; }
        public SnailFishNumber Right { get; private set; }

        public override ReduceOperation Reduce(int depth)
        {
            // Self
            if (depth >= 4) // We go boom
            {
                if (Left is not LiteralSnailFishNumber || Right is not LiteralSnailFishNumber)
                {
                    throw new Oopsie("Cannot explode :D");
                }

                return ReduceOperation.FromExplosion(this);
            }

            // Left
            var leftOperation = Left.Reduce(depth + 1);
            if (leftOperation is not CompletedReduceOperation)
            {
                if (leftOperation is ExplodeReduceOperation expl1)
                {
                    if (expl1.RemoveMe)
                    {
                        Left = new LiteralSnailFishNumber(0);
                        leftOperation = expl1.WithRemovedPerformed();
                    }
                }

                if (leftOperation is ExplodeReduceOperation expl2)
                {
                    if (expl2.AddRight is not null)
                    {
                        if (Right.PerformReduceOperation(expl2.AddRight))
                        {
                            leftOperation = expl2.WithRightPerformed();
                        }
                    }
                }

                return leftOperation;
            }

            // Right
            var rightOperation = Right.Reduce(depth + 1);
            if (rightOperation is not CompletedReduceOperation)
            {
                if (rightOperation is ExplodeReduceOperation expl1)
                {
                    if (expl1.RemoveMe)
                    {
                        Right = new LiteralSnailFishNumber(0);
                        rightOperation = expl1.WithRemovedPerformed();
                    }
                }

                if (rightOperation is ExplodeReduceOperation expl2)
                {
                    if (expl2.AddLeft is not null)
                    {
                        if (Left.PerformReduceOperation(expl2.AddLeft))
                        {
                            rightOperation = expl2.WithLeftPerformed();
                        }
                    }
                }

                return rightOperation;
            }

            return ReduceOperation.Done;
        }

        public override bool PerformReduceOperation(ReduceOperation operation)
        {
            switch (operation)
            {
                case CompletedReduceOperation:
                    return true;
                case AddToLeftMostOperation lmO:
                    {
                        if (Left.PerformReduceOperation(lmO))
                        {
                            return true;
                        }

                        if (Right.PerformReduceOperation(lmO))
                        {
                            return true;
                        }

                        return false;
                    }
                case AddToRightMostOperation rmO:
                    {
                        if (Right.PerformReduceOperation(rmO))
                        {
                            return true;
                        }

                        if (Left.PerformReduceOperation(rmO))
                        {
                            return true;
                        }

                        return false;
                    }
                default: throw new NotImplementedException();
            }
        }

        public override int CalculateMagnitude()
        {
            return 3 * Left.CalculateMagnitude() +
                2 * Right.CalculateMagnitude();
        }
    }

    public class LiteralSnailFishNumber : SnailFishNumber
    {
        public LiteralSnailFishNumber(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }

        public override ReduceOperation Reduce(int depth)
        {
            if (Value > 9)
            {
                var divByTwo = Value / 2d;

                var left = (int)Math.Floor(divByTwo);
                var right = (int)Math.Ceiling(divByTwo);

                return ReduceOperation.FromSplit(new PairSnailFishNumber(new LiteralSnailFishNumber(left), new LiteralSnailFishNumber(right)));
            }

            return ReduceOperation.Done;
        }

        public override int CalculateMagnitude()
        {
            return Value;
        }

        public override bool PerformReduceOperation(ReduceOperation operation)
        {
            switch (operation)
            {
                case CompletedReduceOperation:
                    return true;
                case AddToLeftMostOperation lmO:
                    {
                        Value += lmO.Value;
                        return true;
                    }
                case AddToRightMostOperation rmO:
                    {
                        Value += rmO.Value;
                        return true;
                    }
                default: throw new NotImplementedException();
            }
        }
    }
}
