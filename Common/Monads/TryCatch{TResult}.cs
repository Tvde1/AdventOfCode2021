using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Common.Monads
{
    public readonly struct TryCatch<TResult>
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
                Exception exception = this.Exception;
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }

                return exception;
            }
        }

        public void Match(
            Action<TResult> onSuccess,
            Action<Exception> onError)
        {
            this._either.Match(onLeft: onError, onRight: onSuccess);
        }

        public void ThrowIfFailed()
        {
            if (!this.Succeeded)
            {
                ExceptionDispatchInfo.Capture(this.Exception).Throw();
            }
        }

        public override bool Equals(object? obj)
        {
	        if (obj is TryCatch<TResult> other)
            {
                return Equals(other);
            }

            return false;
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
