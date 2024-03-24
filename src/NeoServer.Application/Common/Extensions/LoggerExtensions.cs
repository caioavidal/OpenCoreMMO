using System.Diagnostics;
using Serilog;

namespace NeoServer.Application.Common.Extensions;

public static class LoggerExtensions
{
    private static readonly Stopwatch Sw = new();

    public static void Step<T>(this ILogger logger, string beforeMessage, string afterMessage, Func<T> func,
        params object[] @params)
    {
        var lastRow = -1;
        var currentRow = -1;

        try
        {
            lastRow = Console.CursorTop;
        }
        catch { }

        if (@params is null || @params.Length == 0)
            logger.Information(beforeMessage);
        else
            logger.Information(beforeMessage, @params);

        Sw.Restart();

        var result = func();

        if(lastRow != -1)
        {
            currentRow = Console.CursorTop;
            Console.SetCursorPosition(0, lastRow);
        }

        if (@params is null || @params.Length == 0)
            logger.Information($"{afterMessage} in {{elapsed}} secs", result,
                Math.Round(Sw.ElapsedMilliseconds / 1000d, 2));
        else
            logger.Information($"{afterMessage} in {{elapsed}} secs",
                @params.Concat(new object[] { Math.Round(Sw.ElapsedMilliseconds / 1000d, 2) }).ToArray());

        if (lastRow != -1)
            Console.SetCursorPosition(0, currentRow);
    }

    public static void Step(this ILogger logger, string beforeMessage, string afterMessage, Action action,
        params object[] @params)
    {
        var lastRow = -1;
        var currentRow = -1;

        try
        {
            lastRow = Console.CursorTop;
        }
        catch { }

        if (@params is null || @params.Length == 0)
            logger.Information(beforeMessage);
        else
            logger.Information(beforeMessage, @params);

        Sw.Restart();

        action();

        if (lastRow != -1)
        {
            currentRow = Console.CursorTop;
            Console.SetCursorPosition(0, lastRow);
        }

        if (@params is null || @params.Length == 0)
            logger.Information($"{afterMessage} in {{elapsed}} secs",
                Math.Round(Sw.ElapsedMilliseconds / 1000d, 2));
        else
            logger.Information($"{afterMessage} in {{elapsed}} secs",
                @params.Concat(new object[] { Math.Round(Sw.ElapsedMilliseconds / 1000d, 2) }).ToArray());

        if (lastRow != -1)
            Console.SetCursorPosition(0, currentRow);
    }

    public static void Step(this ILogger logger, string beforeMessage, string afterMessage, Func<object[]> action)
    {
        var lastRow = -1;
        var currentRow = -1;

        try
        {
            lastRow = Console.CursorTop;
        }
        catch { }

        logger.Information(beforeMessage);

        Sw.Restart();

        var @params = action();

        if (lastRow != -1)
        {
            currentRow = Console.CursorTop;
            Console.SetCursorPosition(0, lastRow);
        }

        if (@params is null || @params.Length == 0)
            logger.Information($"{afterMessage} in {{elapsed}} secs",
                Math.Round(Sw.ElapsedMilliseconds / 1000d, 2));
        else
            logger.Information($"{afterMessage} in {{elapsed}} secs",
                @params.Concat(new object[] { Math.Round(Sw.ElapsedMilliseconds / 1000d, 2) }).ToArray());


        if (lastRow != -1)
            Console.SetCursorPosition(0, currentRow);
    }
}