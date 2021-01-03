using NeoServer.Game.Contracts.Spells;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Spells
{
    public class SpellList
    {
        public static Dictionary<string, ISpell> Spells { get; set; } = new Dictionary<string, ISpell>();
    }
}
