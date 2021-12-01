using System;
using System.Collections.Generic;
using System.IO;
using AdventOfCode.Common.Monads;

namespace AdventOfCode.Common
{
	public class AdventAssignment<TData> : AdventAssignment
	{
		private readonly string _inputFileName;
		private readonly Func<string, TData> _dataParser;
		private readonly Func<TData, IEnumerable<string>> _executeFunc;

		private AdventAssignment(int index, string inputFileName, Func<string, TData> dataParser,
			Func<TData, IEnumerable<string>> executeFunc)
			: base(index)
		{
			_inputFileName = inputFileName;
			_dataParser = dataParser;
			_executeFunc = executeFunc;
		}

		public override IEnumerable<string> Run()
		{
			var tryReadInput = TryCatch.Try(() => File.ReadAllText(_inputFileName));
			if (tryReadInput.Failed)
			{
				yield return "Error: could not find input file.";
				tryReadInput.ThrowIfFailed();
			}

			var input = tryReadInput.Result;

			var tryParseData = TryCatch.Try(() => _dataParser(input));
			if (tryParseData.Failed)
			{
				yield return "Error: data parsing failed.";
				tryParseData.ThrowIfFailed();
			}

			var data = tryParseData.Result;

			var tryExecute = TryCatch.Try(() => _executeFunc(data));
			if (tryExecute.Failed)
			{
				yield return "Error: execution failed.";
				tryExecute.ThrowIfFailed();
			}

			foreach (var line in tryExecute.Result)
			{
				yield return line;
			}
		}

		public static AdventAssignment<TData> Build(int index, string inputFileName, Func<string, TData> dataParser,
			Func<TData, IEnumerable<string>> executeFunc) => new(index, inputFileName, dataParser, executeFunc);
	}
}
