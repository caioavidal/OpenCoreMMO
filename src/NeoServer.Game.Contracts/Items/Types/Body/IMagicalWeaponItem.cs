using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IMagicalWeaponItem : IBodyEquipmentItem
    {
        DamageType DamageType { get; }
        byte AverageDamage { get; }
        byte ManaWasting { get; }
        byte Range { get; }
    }
}
