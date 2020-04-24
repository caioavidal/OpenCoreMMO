using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Items
{
    public interface ILiquidItem : IItem
    {
        bool IsLiquidPool { get; }

        bool IsLiquidSource { get; }

        bool IsLiquidContainer { get; }
        LiquidColor LiquidColor { get; }
    }
}
