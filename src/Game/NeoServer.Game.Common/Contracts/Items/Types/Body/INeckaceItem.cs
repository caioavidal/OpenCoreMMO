using System.Collections.Generic;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Body
{
    public interface INecklaceItem : IChargeable, IDurable, IBodyEquipmentItem
    {
        bool Expired { get; }
        byte Defense { get; }
        Dictionary<DamageType, byte> DamageProtection { get; }
    }
}