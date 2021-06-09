using System;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IAmmoItem : ICumulative, IBodyEquipmentItem
    {
        byte Range { get; }
        byte Attack { get; }
        byte ExtraHitChance { get; }
        AmmoType AmmoType { get; }
        ShootType ShootType { get; }
        bool HasElementalDamage { get; }
        Tuple<DamageType, byte> ElementalDamage { get; }

        void Throw();
    }
}