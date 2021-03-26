using System.Collections.Generic;

namespace NeoServer.Game.Common.Helpers
{
    public static class QueueExtensions
    {
        public static bool IsEmpty<T>(this Queue<T> queue) => queue.Count == 0;
    }
}
