using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;

namespace NeoServer.Game.Common.Contracts.Services;

public interface IPlayerUseService
{
    void Use(IPlayer player, IItem item);
    void Use(IPlayer player, IUsableOn usableItem, IThing usedOn);
    void Use(IPlayer player, IContainer container, byte openAtIndex);
}