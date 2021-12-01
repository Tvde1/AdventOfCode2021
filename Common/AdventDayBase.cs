using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Common
{
	public abstract class AdventDayBase
	{
		private readonly Func<IEnumerable<string>>[] _parts;

		protected AdventDayBase(params Func<IEnumerable<string>>[] parts)
		{
			_parts = parts;
		}

		public IEnumerable<string> Run()
		{
			return _parts.SelectMany(part => part());
		}


	}

	public abstract class AdventAssignment<TData>
	{
		private readonly string _inputFileName;
		private readonly Func<string, TData> _dataParser;
		private readonly Func<TData, IEnumerable<string>> _executeFunc;

		public IEnumerable<string> Run()
		{
			var input = File.ReadAllText(_inputFileName);
		}

	}
}
