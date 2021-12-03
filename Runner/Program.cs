using System;

namespace AdventOfCode.Runner
{
	public class Program
	{
		public static void Main()
		{
			var runner = AdventRunner.Build();
			runner.Run();
			Console.ReadKey();
		}
	}
}