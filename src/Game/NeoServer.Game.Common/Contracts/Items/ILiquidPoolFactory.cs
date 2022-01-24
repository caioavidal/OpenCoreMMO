using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items;

public interface ILiquidPoolFactory : IFactory
{
    ILiquid Create(Location.Structs.Location location, LiquidColor color);
    ILiquid CreateDamageLiquidPool(Location.Structs.Location location, LiquidColor color);
}