using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Helpers.Extensions
{
    public static class LoggerExtensions
    {
        static Stopwatch sw = new Stopwatch();

        public static void Step(this Logger logger, string beforeMessage, string afterMessage, Action action, params object[] @params)
        {
            if (@params is null || @params.Length == 0)
            {
                logger.Information(beforeMessage);
            }
            else
            {
                logger.Information(beforeMessage, @params);
            }
            sw.Restart();

            action();

            Console.SetCursorPosition(0, Console.CursorTop - 1);

            if (@params is null || @params.Length == 0)
            {
                logger.Information($"{afterMessage} in {{elapsed}} secs", Math.Round(sw.ElapsedMilliseconds / 1000d, 2));
            }
            else
            {
                logger.Information($"{afterMessage} in {{elapsed}} secs", @params.Concat(new object[] { Math.Round(sw.ElapsedMilliseconds / 1000d, 2) }).ToArray());
            }

        }
        public static void Step(this Logger logger, string beforeMessage, string afterMessage, Func<object[]> action)
        {
            logger.Information(beforeMessage);
            
            sw.Restart();

            var @params = action();

            Console.SetCursorPosition(0, Console.CursorTop - 1);

            if (@params is null || @params.Length == 0)
            {
                logger.Information($"{afterMessage} in {{elapsed}} secs", Math.Round(sw.ElapsedMilliseconds / 1000d, 2));
            }
            else
            {
                logger.Information($"{afterMessage} in {{elapsed}} secs", @params.Concat(new object[] { Math.Round(sw.ElapsedMilliseconds / 1000d, 2) }).ToArray());
            }

        }
    }
   
}
