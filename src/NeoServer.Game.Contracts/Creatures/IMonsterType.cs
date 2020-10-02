using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IMonsterType : ICreatureType
    {
        ushort Armor { get; set; }
        ushort Defence { get; set; }

        public uint Experience { get; set; }
        public List<ICombatAttack> Attacks { get; set; }
        IDictionary<CreatureFlagAttribute, byte> Flags { get; set; }
    }
}
