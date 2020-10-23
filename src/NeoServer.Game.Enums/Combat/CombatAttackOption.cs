using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Combat.Attacks
{
    public struct CombatAttackOption
    {
        public byte Skill { get; set; }
        public byte Attack { get; set; }
        public ushort Interval { get; set; }
        public byte Chance { get; set; }
        public byte Range { get; set; }
        public byte Radius { get; set; }
        public byte Target { get; set; }
        public ShootType ShootType { get; set; }
        public DamageType DamageType { get; set; }
        public ushort MinDamage { get; set; }
        public ushort MaxDamage { get; set; }
        public byte Spread { get; set; }
        public byte Length { get; set; }
    }
}
