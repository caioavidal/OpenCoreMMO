using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Jobs.Items;

public class LiquidPoolJob
{
    public static void Execute(IDecayable item, IGameServer game)
    {
        if (item is not ILiquid { Expired: true } liquid) return;

        var tile = game.Map[liquid.Location] as IDynamicTile;
        if (liquid.TryDecay()) game.Map.CreateBloodPool(liquid, tile); //todo: need to review this

        if (liquid.ShouldDisappear) game.Map.CreateBloodPool(null, tile);
    }
}