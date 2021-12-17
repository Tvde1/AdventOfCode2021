using System;
using System.IO;

namespace AdventOfCode.Common;

public enum AdventDataSourceType
{
    File,
    Web,
}

public record struct AdventDataSource(AdventDataSourceType Type, string Location)
{
    public static AdventDataSource FromFile(string fileName) => new AdventDataSource(AdventDataSourceType.File, fileName);

    public static AdventDataSource FromWeb(string url) => new AdventDataSource(AdventDataSourceType.Web, url);

    public string GetInput()
    {
        return Type switch
        {
            AdventDataSourceType.File => File.ReadAllText(Location),
            AdventDataSourceType.Web => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}