using AdventOfCode.Common.Monads;
using System.Collections.Generic;
using System.Diagnostics;

namespace AdventOfCode.Common;

public delegate string AdventDayPart<TData>(TData data);

public record struct AdventDayPartResult(string Output, long ExecutionDurationMs);

public record struct AdventDayResult(long ParseDurationMs, AdventDayPartResult[] PartResults);

public interface IAdventDay
{
    AdventDayResult Run();
}

public delegate TData AdventDataParser<TData>(string input);

public class AdventDay<TData> : IAdventDay
{
    private readonly AdventDataSource _dataSource;
    private readonly AdventDataParser<TData> _parser;
    private readonly AdventDayPart<TData>[] _parts;

    public AdventDay(int dayNumber, AdventDataSource dataSource, AdventDataParser<TData> parser, params AdventDayPart<TData>[] parts)
    {
        DayNumber = dayNumber;
        _dataSource = dataSource;
        _parser = parser;
        _parts = parts;
    }

    public int DayNumber { get; }

    public AdventDayResult Run()
    {
        var input = _dataSource.GetInput();

        var sw = new Stopwatch();

        sw.Start();
        var parsed = _parser(input);
        sw.Stop();

        var parsedMs = sw.ElapsedMilliseconds;

        var parseResults = new List<AdventDayPartResult>();
        foreach (var part in _parts)
        {
            sw.Restart();
            var partReuslt = part(parsed);
            sw.Stop();

            var executedMs = sw.ElapsedMilliseconds;
            parseResults.Add(new AdventDayPartResult(partReuslt, executedMs));
        }

        return new AdventDayResult(parsedMs, parseResults.ToArray());
    }
}
