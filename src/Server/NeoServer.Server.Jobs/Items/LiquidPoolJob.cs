using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;

namespace NeoServer.Server.Jobs.Creatures
{
    public class LiquidPoolJob
    {
        public static void Execute(IDecayable item, Game game)
        {
            if (!(item is ILiquid liquid)) return;
            if (item.Expired)
            {
                var tile = game.Map[(item as IItem).Location] as IDynamicTile;
                if (item.Decay())
                {
                    game.Map.CreateBloodPool(liquid, tile);
                }

                if (item.ShouldDisappear)
                {
                    game.Map.CreateBloodPool(null, tile);
                }
            }
        }
    }
}
