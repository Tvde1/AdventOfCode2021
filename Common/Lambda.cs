using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Common.Monads
{
	public class Lambda
	{
		public static Func<T, T> Identity<T>() => x => x;

		public static Action<T> Discard<T>() => _ => { };
	}
}
