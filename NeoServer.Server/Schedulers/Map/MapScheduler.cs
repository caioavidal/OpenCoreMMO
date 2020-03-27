using System;
using System.Collections.Concurrent;
using System.Threading;
using NeoServer.Game.Contracts;

namespace NeoServer.Server.Schedulers.Map
{
    public class MapScheduler : Scheduler
    {
        private readonly IMap _map;

        public MapScheduler(IMap map)
        {
            _map = map;
        }
    }
}