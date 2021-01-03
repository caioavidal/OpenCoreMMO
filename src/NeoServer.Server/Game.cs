using NeoServer.Game.Contracts;
using NeoServer.Server.Contracts.Tasks;
using NeoServer.Server.Instances;

namespace NeoServer.Server
{
    public class Game
    {
        /// <summary>
        /// Game's light level
        /// </summary>
        public byte LightLevel => 250;

        /// <summary>
        /// Indicates Game's light color
        /// </summary>
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
        public DecayableItemManager DecayableItemManager { get; }

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
            IDispatcher dispatcher, IScheduler scheduler, GameCreatureManager creatureManager, DecayableItemManager decayableBag)
        {

            Map = map;
            Dispatcher = dispatcher;
            Scheduler = scheduler;
            CreatureManager = creatureManager;
            DecayableItemManager = decayableBag;
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
