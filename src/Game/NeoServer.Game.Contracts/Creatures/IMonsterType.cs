using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Combat.Attacks;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IMonsterType : ICreatureType
    {
        ushort Armor { get; set; }
        ushort Defense { get; set; }

        public uint Experience { get; set; }
        public IMonsterCombatAttack[] Attacks { get; set; }
        public ICombatDefense[] Defenses { get; set; }

        IDictionary<CreatureFlagAttribute, ushort> Flags { get; set; }
        IIntervalChance TargetChance { get; set; }

        /// <summary>
        /// Monster's phases
        /// </summary>
        string[] Voices { get; set; }
        /// <summary>
        /// Voice interval and chance to happen
        /// </summary>
        IIntervalChance VoiceConfig { get; set; }
        ImmutableDictionary<DamageType, sbyte> Immunities { get; set; }
        Race Race { get; set; }
        ILoot Loot { get; set; }
    }
}
