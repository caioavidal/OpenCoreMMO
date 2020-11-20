using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Items
{
    public interface ILiquidPoolFactory: IFactory
    {
        ILiquid Create(Location location, LiquidColor color);
        ILiquid CreateDamageLiquidPool(Location location, LiquidColor color);
    }
}
