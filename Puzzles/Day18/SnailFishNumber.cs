using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AdventOfCode.Puzzles.Day18
{
    public abstract class SnailFishNumber
    {
        public void ReduceFull(out string steps)
        {
            steps = "";
            var i = 1;
            while (true)
            {
                var explodeOperation = ReduceInternal(0, ReduceStrategy.Explosions);
                if (explodeOperation is not NoActionReduceOperation)
                {
                    steps += $"After step {i:D3} ({explodeOperation,18}): {this}{Environment.NewLine}";
                    continue;
                }

                var splitOperation = ReduceInternal(0, ReduceStrategy.Splits);
                if (splitOperation is not NoActionReduceOperation)
                {
                    steps += $"After step {i:D3} ({splitOperation,18}): {this}{Environment.NewLine}";
                    continue;
                }

                break;
            }
        }

        public abstract bool PerformReduceOperation(ReduceOperation operation);

        public abstract int CalculateMagnitude();

        public abstract SnailFishNumber Clone();

        public static SnailFishNumber Add(SnailFishNumber left, SnailFishNumber right) 
        {
            return new PairSnailFishNumber(left.Clone(), right.Clone());
        }

        public static SnailFishNumber Parse(string input)
        {
            var parsed = ParseInternal(new Queue<char>(input));

            Debug.Assert(parsed.ToString() == input);

            return parsed;
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

        public abstract override string ToString();

        public abstract ReduceOperation ReduceInternal(int depth, ReduceStrategy strategy);
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

        public override ReduceOperation ReduceInternal(int depth, ReduceStrategy strategy)
        {
            if (strategy == ReduceStrategy.Explosions)
            {
                // Self
                if (depth >= 4) // We go boom
                {
                    if (Left is LiteralSnailFishNumber && Right is LiteralSnailFishNumber)
                    {
                        return ReduceOperation.FromExplosion(this);
                    }
                }
            }

            // Left
            {
                var leftOperation = Left.ReduceInternal(depth + 1, strategy);
                if (leftOperation is not NoActionReduceOperation)
                {
                    if (strategy == ReduceStrategy.Explosions)
                    {
                        if (leftOperation is ExplodeReduceOperation expl1)
                        {
                            if (expl1.RemoveMe)
                            {
                                Left = new LiteralSnailFishNumber(0);
                                leftOperation = expl1.WithRemovedPerformed();
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
                        }
                    }
                    else if (strategy == ReduceStrategy.Splits)
                    {
                        if (leftOperation is SplitReduceOperation spl)
                        {
                            Left = spl.ReplacePair;

                            leftOperation = spl.WithReplaced();
                        }
                    }

                    return leftOperation;
                }
            }

            // Right
            {
                var rightOperation = Right.ReduceInternal(depth + 1, strategy);
                if (rightOperation is not NoActionReduceOperation)
                {
                    if (strategy == ReduceStrategy.Explosions)
                    {
                        if (rightOperation is ExplodeReduceOperation expl1)
                        {
                            if (expl1.RemoveMe)
                            {
                                Right = new LiteralSnailFishNumber(0);
                                rightOperation = expl1.WithRemovedPerformed();
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
                        }
                    }
                    else if (strategy == ReduceStrategy.Splits)
                    {
                        if (rightOperation is SplitReduceOperation spl)
                        {
                            Right = spl.ReplacePair;

                            rightOperation = spl.WithReplaced();
                        }
                    }

                    return rightOperation;
                }
            }

            return ReduceOperation.NoAction;
        }

        public override bool PerformReduceOperation(ReduceOperation operation)
        {
            switch (operation)
            {
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
            return (3 * Left.CalculateMagnitude()) + (2 * Right.CalculateMagnitude());
        }

        public override string ToString()
        {
            return $"[{Left},{Right}]";
        }

        public override SnailFishNumber Clone()
        {
            return new PairSnailFishNumber(Left.Clone(), Right.Clone());
        }
    }

    public class LiteralSnailFishNumber : SnailFishNumber
    {
        public LiteralSnailFishNumber(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }

        public override ReduceOperation ReduceInternal(int depth, ReduceStrategy strategy)
        {
            if (strategy == ReduceStrategy.Splits && Value > 9)
            {
                var divByTwo = Value / 2d;

                var left = (int)Math.Floor(divByTwo);
                var right = (int)Math.Ceiling(divByTwo);

                return ReduceOperation.FromSplit(new PairSnailFishNumber(new LiteralSnailFishNumber(left), new LiteralSnailFishNumber(right)));
            }

            return ReduceOperation.NoAction;
        }

        public override int CalculateMagnitude()
        {
            return Value;
        }

        public override bool PerformReduceOperation(ReduceOperation operation)
        {
            switch (operation)
            {
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

        public override string ToString()
        {
            return Value.ToString();
        }

        public override SnailFishNumber Clone()
        {
            return new LiteralSnailFishNumber(Value);
        }
    }
}
