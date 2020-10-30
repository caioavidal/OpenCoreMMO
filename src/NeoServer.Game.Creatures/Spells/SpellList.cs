using NeoServer.Game.Contracts.Spells;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Spells
{
    public class SpellList
    {
        public static Dictionary<string, ISpell> Spells { get; set; } = new Dictionary<string, ISpell>();
    }
}
