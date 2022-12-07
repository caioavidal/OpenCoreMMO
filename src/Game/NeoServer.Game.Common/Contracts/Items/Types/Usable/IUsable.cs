using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Items.Types.Usable;

public interface IUsable
{
    void Use(IPlayer usedBy);
}