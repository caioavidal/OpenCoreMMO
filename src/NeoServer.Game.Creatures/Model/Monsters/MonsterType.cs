using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Model.Monsters
{
    public sealed class MonsterType: IMonsterType
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Race Race { get; set; }
        public uint Experience { get; set; }
        public ushort Speed { get; set; }
        public ushort ManaCost { get; set; }
        public uint MaxHealth { get; set; }
        public IDictionary<LookType, ushort> Look { get; set; }
        public TargetChance TargetChance { get; set; }
        public CombatStrategy CombatStrategy { get; set; }
        public IDictionary<CreatureFlagAttribute, byte> Flags { get; set; }
        public IDictionary<DamageType, CombatAttack> Attacks { get; set; }
        public ushort Armor { get; set; }
        public ushort Defence { get; set; }
    }
}
