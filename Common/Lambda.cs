using System;

namespace AdventOfCode.Common
{
	public class Lambda
	{
		public static Func<T, T> Identity<T>() => x => x;

		public static Action<T> Discard<T>() => _ => { };
	}
}
