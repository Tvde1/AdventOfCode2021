using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AdventOfCode.Common;

public enum AdventDataSourceType
{
    File,
    Web,
    Raw,
}

public record struct AdventDataSource(AdventDataSourceType Type, string Location)
{
    public static AdventDataSource FromFile(string fileName) => new(AdventDataSourceType.File, fileName);

    public static AdventDataSource ForThisDay([CallerFilePath] string caller = "")
    {

        // C:\Repos\AoC\Puzzles\2021\Day01\Day1.cs

        var fileName = caller.Split("Puzzles\\")[1].Replace("cs", "txt");

        return new AdventDataSource(AdventDataSourceType.File, fileName);
    }

    public static AdventDataSource FromWeb(string url) => new(AdventDataSourceType.Web, url);

    public static AdventDataSource FromRaw(string data) => new(AdventDataSourceType.Raw, data);

    public readonly string GetInput()
    {
        return Type switch
        {
            AdventDataSourceType.File => File.ReadAllText(Location),
            AdventDataSourceType.Web => throw new NotImplementedException(),
            AdventDataSourceType.Raw => Location,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}