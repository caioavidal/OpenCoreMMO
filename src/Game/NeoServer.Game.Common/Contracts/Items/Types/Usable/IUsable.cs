using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Items.Types.Usable;

public interface IUsable : IItem
{
    void Use(IPlayer player);
}