using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface INpcType:ICreatureType
    {
        public Dictionary<string, INpcDialog> Dialog { get; set; }
    }

    public interface INpcDialog
    {
        public string[] OnWords { get; set; }
        public string[] Answer { get; set; }
        public string[] OnDecline { get; set; }
        public string Exec { get; set; }
        public INpcDialog Then { get; set; }
    }
}
