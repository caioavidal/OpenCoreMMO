using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Jobs.Items
{
    public class LiquidPoolJob
    {
        public static void Execute(IDecayable item, IGameServer game)
        {
            if (!(item is ILiquid liquid)) return;
            if (item.Expired)
            {
                var tile = game.Map[(item as IItem).Location] as IDynamicTile;
                if (item.TryDecay()) game.Map.CreateBloodPool(liquid, tile); //todo: need to review this

                if (item.ShouldDisappear) game.Map.CreateBloodPool(null, tile);
            }
        }
    }
}