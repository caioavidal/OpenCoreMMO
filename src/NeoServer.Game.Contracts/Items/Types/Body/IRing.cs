using NeoServer.Game.Enums.Item;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IRing : IChargeable, IDurable, IBodyEquipmentItem
    {
        bool Expired { get; }
        byte Defense { get; }
        Dictionary<DamageType, byte> DamageProtection { get; }

    }
}
