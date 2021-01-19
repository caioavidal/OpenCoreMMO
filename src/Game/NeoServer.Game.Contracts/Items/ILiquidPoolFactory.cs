using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Contracts.Items
{
    public interface ILiquidPoolFactory: IFactory
    {
        ILiquid Create(Location location, LiquidColor color);
        ILiquid CreateDamageLiquidPool(Location location, LiquidColor color);
    }
}
