using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Common.Monads
{
	public readonly record struct TryCatch
	{
		public static TryCatch<T> Try<T>(Func<T> func)
		{
			return TryCatch<T>.Try(func);
		}
	}

    public readonly record struct TryCatch<TResult>
    {
        private readonly Either<Exception, TResult> _either;

        private TryCatch(Either<Exception, TResult> either)
        {
            _either = either;
        }

        public bool Succeeded => _either.IsRight;

        public bool Failed => !Succeeded;

        public TResult Result =>
            _either.Match(l => throw new InvalidOperationException(
                    $"Tried to get the result of a {nameof(TryCatch<TResult>)} that ended in an exception."),
                r => r);

        public Exception Exception =>
            _either.Match(e => e, r => throw new InvalidOperationException(
                $"Tried to get the exception of a {nameof(TryCatch<TResult>)} that ended in a result."));

        public Exception InnerMostException
        {
            get
            {
                Exception exception = Exception;
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }

                return exception;
            }
        }

        public TResult ResultOrThrow
        {
            get
            {
                ThrowIfFailed();
                return Result;
            }
        }

        public void Match(
            Action<TResult> onSuccess,
            Action<Exception> onError)
        {
            _either.Match(onError, onSuccess);
        }

        public void ThrowIfFailed()
        {
            if (!Succeeded)
            {
                ExceptionDispatchInfo.Capture(Exception).Throw();
            }
        }

        public TryCatch<T> Continue<T>(Func<TResult, T> onSucceeded)
        {
            return _either.Match(TryCatch<T>.FromException, r => TryCatch.Try(() => onSucceeded(r)));
        }

        public TryCatch<TResult> SelectException<TEx>(Func<TEx, Exception> selector)
        {
            return this switch
            {
                { Succeeded: true } => this,
                { Exception: TEx e } => FromException(selector(e)),
                _ => this,
            };
        }

        public bool Equals(TryCatch<TResult> other)
        {
            return _either.Equals(other._either);
        }

        public override int GetHashCode()
        {
            return _either.GetHashCode();
        }

        public static TryCatch<TResult> FromResult(TResult result)
        {
            return new TryCatch<TResult>(result);
        }

        public static TryCatch<TResult> FromException(Exception exception)
        {
            return new TryCatch<TResult>(
                new AggregateException(
                    $"{nameof(TryCatch<TResult>)} resulted in an exception.",
                    exception));
        }

        public static TryCatch<TResult> Try(Func<TResult> func)
        {
            try
            {
                return FromResult(func());
            }
            catch (Exception ex)
            {
                return FromException(ex);
            }
        }
    }
}
