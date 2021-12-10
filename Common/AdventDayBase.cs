using System.Collections.Generic;
using System.Diagnostics;

namespace AdventOfCode.Common
{
	public abstract class AdventDayBase
	{
		private readonly List<AdventAssignment> _parts = new();

		protected AdventDayBase(int number)
		{
            Number = number;
		}

        public int Number { get; }

        protected AdventDayBase AddPart(AdventAssignment part)
		{
			_parts.Add(part);
			return this;
		}

		public IEnumerable<string> Run()
		{
			var sw = new Stopwatch();
			yield return "==========";
			yield return $"Begin day {Number}";

			var index = 0;
			foreach (var part in _parts)
			{
				yield return $"Part: {++index}";

				sw.Start();
				var lines = part.Run();
				sw.Stop();
				foreach (var line in lines) yield return line;
				yield return $"Took {sw.Elapsed.TotalMilliseconds}ms";
			}
		}
	}
}