using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;

using NeoServer.Server.Tasks.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NeoServer.Server
{
    public class Game
    {
        public byte LightLevel => 250;
        public byte LightColor => 215;

        public GameState State { get; private set; }


        public IMap Map { get; }

        public GameCreatureManager CreatureManager { get; }

        public IDispatcher Dispatcher { get; }
        public IScheduler Scheduler { get; }

        public Game(IMap map,
            IDispatcher dispatcher, IScheduler scheduler, GameCreatureManager creatureManager)
        {

            Map = map;
            Dispatcher = dispatcher;
            Scheduler = scheduler;
            CreatureManager = creatureManager;
        }
      
        /// <summary>
        /// Removes player from game instances, connection pool and close player connection;
        /// </summary>
     

        public void Open() => State = GameState.Opened;
        public void Close() => State = GameState.Closed;

        public DateTime CombatSynchronizationTime { get; private set; }
    }
}
