using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Enums.Item;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IMonsterType : ICreatureType
    {
        ushort Armor { get; set; }
        ushort Defence { get; set; }

        public uint Experience { get; set; }
        IDictionary<DamageType, ICombatAttack> Attacks { get; set; }
    }
}
