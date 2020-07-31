using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IRing : IChargeable, IDurable, IBodyEquipmentItem
    {
        bool Expired { get; }
        byte Defense { get; }
        Dictionary<DamageType, byte> DamageProtection { get; }

    }
}
