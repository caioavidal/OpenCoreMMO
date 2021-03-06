﻿using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Body
{
    public interface IMagicalWeaponItem : IBodyEquipmentItem
    {
        DamageType DamageType { get; }
        byte AverageDamage { get; }
        byte ManaWasting { get; }
        byte Range { get; }
    }
}