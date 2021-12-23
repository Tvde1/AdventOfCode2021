using AdventOfCode.Common.Monads;
using System.Collections.Generic;
using System.Diagnostics;

namespace AdventOfCode.Common;

public delegate string AdventDayPart<TData>(TData data);

public record struct AdventDayPartResult(string Output, long ExecutionDurationMs);

public record struct AdventDayResult(long ParseDurationMs, AdventDayPartResult[] PartResults);

public interface IAdventDayImplementation
{
    AdventDayResult Run();
}

public delegate TData AdventDataParser<TData>(string input);

public static class AdventDayImplementation
{
    public static IAdventDayImplementation Build<TData>(AdventDataSource dataSource,
        AdventDataParser<TData> parser, 
        params AdventDayPart<TData>[] parts)
    {
        return new AdventDayImplementation<TData>(dataSource, parser, parts);
    }
}

public class AdventDayImplementation<TData> : IAdventDayImplementation
{
    private readonly AdventDataSource _dataSource;
    private readonly AdventDataParser<TData> _parser;
    private readonly AdventDayPart<TData>[] _parts;

    public AdventDayImplementation(AdventDataSource dataSource, AdventDataParser<TData> parser, params AdventDayPart<TData>[] parts)
    {
        _dataSource = dataSource;
        _parser = parser;
        _parts = parts;
    }

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
            var partResult = part(parsed);
            sw.Stop();

            var executedMs = sw.ElapsedMilliseconds;
            parseResults.Add(new AdventDayPartResult(partResult, executedMs));
        }

        return new AdventDayResult(parsedMs, parseResults.ToArray());
    }
}

public abstract class AdventDay
{
    public AdventDay(int dayNumber, IAdventDayImplementation implementation)
    {
        DayNumber = dayNumber;
        Implementation = implementation;
    }

    public int DayNumber { get; }

    public IAdventDayImplementation Implementation { get; }
}

