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

        /// <summary>
        /// Game state
        /// </summary>
        /// <value></value>
        public GameState State { get; private set; }

        /// <summary>
        /// Map instance
        /// </summary>
        /// <value></value>
        public IMap Map { get; }

        public GameCreatureManager CreatureManager { get; }

        /// <summary>
        /// Dispatcher instance
        /// </summary>
        /// <value></value>
        public IDispatcher Dispatcher { get; }

        /// <summary>
        /// Scheduler instance
        /// </summary>
        /// <value></value>
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
        /// Sets game state as opened
        /// </summary>
        public void Open() => State = GameState.Opened;

        /// <summary>
        /// Sets game state as closed
        /// No one can logIn on game expect GM
        /// </summary>
        public void Close() => State = GameState.Closed;
    }
}
