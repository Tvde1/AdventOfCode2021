using System;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Common.Monads
{
	public readonly struct TryCatch
	{
		private readonly TryCatch<Void> _voidTryCatch;

		private TryCatch(TryCatch<Void> voidTryCatch)
		{
			this._voidTryCatch = voidTryCatch;
		}

		public Exception Exception => this._voidTryCatch.Exception;

		public bool Succeeded => this._voidTryCatch.Succeeded;

		public bool Failed => this._voidTryCatch.Failed;

		public void Match(
			Action onSuccess,
			Action<Exception> onError) => this._voidTryCatch.Match(
				onSuccess: (dummy) => { onSuccess(); },
				onError: onError);

		public void ThrowIfFailed() => this._voidTryCatch.ThrowIfFailed();

		public override bool Equals(object? obj)
		{
			if (obj is TryCatch other)
			{
				return this.Equals(other);
			}

			return false;
		}

		public bool Equals(TryCatch other) => this._voidTryCatch.Equals(other._voidTryCatch);

		public override int GetHashCode() => this._voidTryCatch.GetHashCode();

		public static TryCatch FromResult() => new(TryCatch<Void>.FromResult(default));

		public static TryCatch FromException(Exception exception) => new(TryCatch<Void>.FromException(exception));

		public static TryCatch Try(Action action)
		{
			try
			{
				action();
				return TryCatch.FromResult();
			}
			catch (Exception ex)
			{
				return TryCatch.FromException(ex);
			}
		}

		public static TryCatch<T> Try<T>(Func<T> func)
		{
			return TryCatch<T>.Try(func);
		}


		/// <summary>
		/// Represents a void return type.
		/// </summary>
		private readonly struct Void
		{
		}

		public static bool operator ==(TryCatch left, TryCatch right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(TryCatch left, TryCatch right)
		{
			return !(left == right);
		}
	}
}
