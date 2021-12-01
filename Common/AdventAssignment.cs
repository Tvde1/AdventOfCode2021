using System;
using System.Collections.Generic;

namespace AdventOfCode.Common
{
	public abstract class AdventAssignment
	{
		protected AdventAssignment(int index)
		{
			Index = index;
		}

		public abstract IEnumerable<string> Run();

		public int Index { get; }

		public static AdventAssignment Build<TData>(int index, string inputFileName, Func<string, TData> dataParser,
			Func<TData, IEnumerable<string>> executeFunc) =>
			AdventAssignment<TData>.Build(index, inputFileName, dataParser, executeFunc);
	}
}
