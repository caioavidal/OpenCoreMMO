using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World.Tiles;

namespace NeoServer.Game.Common.Contracts.Services;

public interface IPlayerUseService
{
    void Use(IPlayer player, IUsable item);
    void Use(IPlayer player, IUsableOn item, IThing destinationThing);
}