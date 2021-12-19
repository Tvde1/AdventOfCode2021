using System;

namespace AdventOfCode.Common.Monads
{
	public readonly record struct Either<TLeft, TRight>
    {
        private readonly TLeft? _left;
        private readonly TRight? _right;

        private Either(TLeft? left, TRight? right, bool isLeft)
        {
            _left = left;
            _right = right;
            IsLeft = isLeft;
        }

        public bool IsLeft { get; }

        public bool IsRight => !this.IsLeft;

        public void Match(Action<TLeft> onLeft, Action<TRight> onRight)
        {
            if (IsLeft)
            {
                onLeft(_left!);
            }
            else
            {
                onRight(_right!);
            }
        }

        public Either<TLeftOut, TRightOut> Match<TLeftOut, TRightOut>(Func<TLeft, TLeftOut> onLeft, Func<TRight, TRightOut> onRight)
        {
            return IsLeft ? onLeft(_left!) : onRight(_right!);
        }

        public TResult Match<TResult>(Func<TLeft, TResult> onLeft, Func<TRight, TResult> onRight)
        {
            return IsLeft ? onLeft(_left!) : onRight(_right!);
        }

        public static implicit operator Either<TLeft, TRight>(TLeft left)
        {
            return new Either<TLeft, TRight>(
                left,
                default,
                true);
        }

        public static implicit operator Either<TLeft, TRight>(TRight right)
        {
            return new Either<TLeft, TRight>(
                default,
                right,
                false);
        }
    }
}
