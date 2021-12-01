using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Common.Monads;

namespace AdventOfCode.Common
{
	public abstract class AdventDayBase
	{
		private readonly int _day;
		private readonly List<AdventAssignment> _parts = new();

		protected AdventDayBase(int day)
		{
			_day = day;
		}

		protected AdventDayBase AddPart(AdventAssignment part)
		{
			_parts.Add(part);
			return this;
		}

		public IEnumerable<string> Run()
		{
			yield return "==========";
			yield return $"Begin day {_day}";

			foreach (var part in _parts)
			{
				yield return $"Part: {part.Index}";
				var lines = part.Run();
				foreach (var line in lines)
				{
					yield return line;
				}
			}

			yield return "==========";
		}
	}

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
