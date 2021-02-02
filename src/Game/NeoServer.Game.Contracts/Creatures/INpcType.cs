using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface INpcType:ICreatureType
    {
        public INpcDialog[] Dialog { get; init; }
    }

    public interface INpcDialog
    {
        public string[] OnWords { get; init; }
        public string[] Answers { get; init; }
        public string Exec { get; init; }
        public bool End { get; init; }
        public INpcDialog[] Then { get; init; }
    }
}
