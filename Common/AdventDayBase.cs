using System.Collections.Generic;
using System.Linq;

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
		}
	}
}
