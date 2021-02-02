using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Npcs
{
    public sealed class NpcType : INpcType
    {
        public Dictionary<string, INpcDialog> Dialog { get; set; }

        public string Name { get; set; }

        public string Description { get; }

        public uint MaxHealth { get; set; }

        public ushort Speed { get; set; }

        public IDictionary<LookType, ushort> Look { get; set; }
    }
}
