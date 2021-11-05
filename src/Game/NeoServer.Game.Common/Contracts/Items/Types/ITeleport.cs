using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Items.Types
{
    public interface ITeleport
    {
        bool Teleport(IWalkableCreature player);
        bool HasDestination { get; }
    }
}