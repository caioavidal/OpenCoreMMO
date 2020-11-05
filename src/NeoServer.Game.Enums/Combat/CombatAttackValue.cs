using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums.Combat
{
    public struct CombatAttackValue
    {
        public ShootType ShootType { get; set; }

        public Location.Structs.Location[] AffectedArea { get; set; }
        public ushort MinDamage { get; set; }
        public ushort MaxDamage { get; set; }
    }

    public struct CombatDamage
    {
        public ushort Damage { get; set; }
        public DamageType DamageType { get; set; }
    }
}
