using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Common.Monads;

namespace AdventOfCode.Common
{
	public class AdventAssignment<TData> : AdventAssignment
	{
		private readonly string _inputFileName;
		private readonly Func<string, TData> _dataParser;
		private readonly Func<TData, IEnumerable<string>> _executeFunc;

		private AdventAssignment(string inputFileName, Func<string, TData> dataParser,
			Func<TData, IEnumerable<string>> executeFunc)
		{
			_inputFileName = inputFileName;
			_dataParser = dataParser;
			_executeFunc = executeFunc;
		}

		public override List<string> Run()
		{
			return TryCatch.Try(() => File.ReadAllText(_inputFileName))
				.Continue(input => _dataParser(input))
				.Continue(data => _executeFunc(data))
				.ResultOrThrow
				.ToList();
		}

		public static AdventAssignment<TData> Build(string inputFileName, Func<string, TData> dataParser,
			Func<TData, IEnumerable<string>> executeFunc) => new(inputFileName, dataParser, executeFunc);
	}
}
