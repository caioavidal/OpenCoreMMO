using NeoServer.Application.Common.Contracts.Tasks;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Server.Common.Enums;

namespace NeoServer.Application.Common.Contracts;

public delegate void OpenServer();

public interface IGameServer
{
    IGameCreatureManager CreatureManager { get; }
    IMap Map { get; }
    IDispatcher Dispatcher { get; }
    IScheduler Scheduler { get; }
    GameState State { get; }
    byte LightLevel { get; }
    byte LightColor { get; }
    IPersistenceDispatcher PersistenceDispatcher { get; }
    void Close();
    void Open();
    event OpenServer OnOpened;
}