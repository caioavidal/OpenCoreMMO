﻿using System;
using System.Diagnostics;
using System.Linq;
using Serilog.Core;

namespace NeoServer.Server.Helpers.Extensions
{
    public static class LoggerExtensions
    {
        private static readonly Stopwatch sw = new();

        public static void Step<T>(this Logger logger, string beforeMessage, string afterMessage, Func<T> func,
            params object[] @params)
        {
            var lastRow = Console.CursorTop;

            if (@params is null || @params.Length == 0)
                logger.Information(beforeMessage);
            else
                logger.Information(beforeMessage, @params);

            sw.Restart();

            var result = func();

            var currentRow = Console.CursorTop;
            Console.SetCursorPosition(0, lastRow);
            if (@params is null || @params.Length == 0)
                logger.Information($"{afterMessage} in {{elapsed}} secs", result,
                    Math.Round(sw.ElapsedMilliseconds / 1000d, 2));
            else
                logger.Information($"{afterMessage} in {{elapsed}} secs",
                    @params.Concat(new object[] { Math.Round(sw.ElapsedMilliseconds / 1000d, 2) }).ToArray());

            Console.SetCursorPosition(0, currentRow);
        }

        public static void Step(this Logger logger, string beforeMessage, string afterMessage, Action action,
            params object[] @params)
        {
            var lastRow = Console.CursorTop;

            if (@params is null || @params.Length == 0)
                logger.Information(beforeMessage);
            else
                logger.Information(beforeMessage, @params);

            sw.Restart();

            action();

            var currentRow = Console.CursorTop;
            Console.SetCursorPosition(0, lastRow);
            if (@params is null || @params.Length == 0)
                logger.Information($"{afterMessage} in {{elapsed}} secs",
                    Math.Round(sw.ElapsedMilliseconds / 1000d, 2));
            else
                logger.Information($"{afterMessage} in {{elapsed}} secs",
                    @params.Concat(new object[] {Math.Round(sw.ElapsedMilliseconds / 1000d, 2)}).ToArray());

            Console.SetCursorPosition(0, currentRow);
        }

        public static void Step(this Logger logger, string beforeMessage, string afterMessage, Func<object[]> action)
        {
            var lastRow = Console.CursorTop;

            logger.Information(beforeMessage);

            sw.Restart();

            var @params = action();

            var currentRow = Console.CursorTop;
            Console.SetCursorPosition(0, lastRow);

            if (@params is null || @params.Length == 0)
                logger.Information($"{afterMessage} in {{elapsed}} secs",
                    Math.Round(sw.ElapsedMilliseconds / 1000d, 2));
            else
                logger.Information($"{afterMessage} in {{elapsed}} secs",
                    @params.Concat(new object[] {Math.Round(sw.ElapsedMilliseconds / 1000d, 2)}).ToArray());

            Console.SetCursorPosition(0, currentRow);
        }
    }
}