using System;
using System.Collections.Generic;

namespace AdventOfCode.Common
{
	public abstract class AdventAssignment
	{
		public abstract IEnumerable<string> Run();

        public static AdventAssignment Build<TData>(string inputFileName, Func<string, TData> dataParser,
            Func<TData, IEnumerable<string>> executeFunc) =>
            AdventAssignment<TData>.Build(inputFileName, dataParser, executeFunc);

        public static AdventAssignment Build<TData, TOut>(string inputFileName, Func<string, TData> dataParser,
            Func<TData, TOut> executeFunc) =>
            AdventAssignment<TData>.Build(inputFileName, dataParser, x => executeFunc(x)!.ToString()!.Enumerate());
	}
}
