using System;

namespace AdventOfCode.Common.Monads
{
	internal readonly struct Either<TLeft, TRight>
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

        public TResult Match<TResult>(Func<TLeft, TResult> onLeft, Func<TRight, TResult> onRight)
        {
            return IsLeft ? onLeft(_left!) : onRight(_right!);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Either<TLeft, TRight> other)
            {
                return Equals(other);
            }

            return false;
        }

        public bool Equals(Either<TLeft, TRight> other)
        {
	        return IsLeft == other.IsLeft && (IsLeft ? _left!.Equals(other._left) : _right!.Equals(other._right));
        }

        public override int GetHashCode()
        {
	        var hashCode = 0;
            hashCode ^= IsLeft.GetHashCode();
            if (IsLeft)
            {
                hashCode ^= _left!.GetHashCode();
            }
            else
            {
                hashCode ^= _right!.GetHashCode();
            }

            return hashCode;
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
