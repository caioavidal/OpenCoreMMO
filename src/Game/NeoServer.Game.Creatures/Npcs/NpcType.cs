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
        public INpcDialog[] Dialog { get; init; }

        public string Name { get; set; }

        public string Description { get; }

        public uint MaxHealth { get; set; }

        public ushort Speed { get; set; }

        public IDictionary<LookType, ushort> Look { get; set; }
    }

    public sealed class NpcDialogType: INpcDialog
    {
        public string[] OnWords { get ; init; }
        public string[] Answers { get ; init; }
        public string Exec { get; init; }
        public bool End { get; init; }
        public INpcDialog[] Then { get; init; }
    }
}
