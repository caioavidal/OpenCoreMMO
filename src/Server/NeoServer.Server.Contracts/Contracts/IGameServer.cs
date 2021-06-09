using NeoServer.Game.Contracts;
using NeoServer.Server.Contracts.Tasks;

namespace NeoServer.Server.Contracts
{
    public interface IGameServer
    {
        IGameCreatureManager CreatureManager { get; }
        IMap Map { get; }
        IDispatcher Dispatcher { get; }
        IScheduler Scheduler { get; }
        IDecayableItemManager DecayableItemManager { get; }
        GameState State { get; }
        byte LightLevel { get; }
        byte LightColor { get; }

        void Close();
        void Open();
    }
}