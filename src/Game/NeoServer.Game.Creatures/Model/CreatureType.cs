using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Contracts.Creatures;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Model
{
    public class CreatureType : ICreatureType
    {
        public CreatureType(string name, string description, uint maxHealth, ushort speed, IDictionary<LookType, ushort> look)
        {
            Name = name;
            Description = description;
            MaxHealth = maxHealth;
            Speed = speed;
            Look = look;
        }

        public string Name { get; }
        public string Description { get; }
        public uint MaxHealth { get; private set; }
        public ushort Speed { get; set; }
        public IDictionary<LookType, ushort> Look { get; private set; }
    }
}
