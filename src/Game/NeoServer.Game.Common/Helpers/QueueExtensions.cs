using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Helpers
{
    public static class QueueExtensions
    {
        public static bool IsEmpty<T>(this Queue<T> queue) => queue.Count == 0;
    }
}
