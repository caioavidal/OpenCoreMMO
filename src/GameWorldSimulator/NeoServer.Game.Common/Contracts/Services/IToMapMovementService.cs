using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Structs;

namespace NeoServer.Game.Common.Contracts.Services;

public interface IToMapMovementService
{
    void Move(IPlayer player, MovementParams itemThrow);
}