using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IPickupableItem : IMoveableThing, IItem
    {
        float Weight { get; }
    }
}
